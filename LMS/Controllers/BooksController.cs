//using Microsoft.AspNetCore.Mvc;

//namespace LMS.Controllers
//{
//    public class BooksController
//    {
//    }
//}

using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryDbContext _context;
        public BooksController(LibraryDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.Include(b => b.Author).OrderBy(b => b.Title).ToListAsync();
            return View(books);
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BorrowRecords)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) return NotFound();
            return View(book);
        }

        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (book.TotalCopies < 1)
                ModelState.AddModelError(nameof(book.TotalCopies), "TotalCopies must be ≥ 1");

            book.AvailableCopies = book.TotalCopies;

            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            ViewBag.Authors = _context.Authors.ToList();
            return View(book);
        }

        public async Task<IActionResult> Borrow(int BookId)
        {
            var book = await _context.Books.FindAsync(BookId);
            if (book == null) return NotFound();

            var record = new BorrowRecord { BookId = BookId, DueDate = DateTime.Now.AddDays(14) };
            ViewBag.BookTitle = book.Title;
            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrow(BorrowRecord record)
        {
            var book = await _context.Books.FindAsync(record.BookId);
            if (book == null) return NotFound();

            if (book.AvailableCopies < 1)
                ModelState.AddModelError("", "No copies available");

            if (ModelState.IsValid)
            {
                book.AvailableCopies -= 1;
                _context.BorrowRecords.Add(record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = record.BookId });
            }

            ViewBag.BookTitle = book.Title;
            return View(record);
        }
    }
}