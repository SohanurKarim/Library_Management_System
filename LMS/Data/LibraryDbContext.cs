using LMS.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();

            modelBuilder.Entity<Book>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Book_AvailableCopies",
                    "[AvailableCopies] <= [TotalCopies]"
                ));

            modelBuilder.Entity<BorrowRecord>()
                .Property(b => b.BorrowDate)
                .HasDefaultValueSql("GETDATE()");

            base.OnModelCreating(modelBuilder);
        }
    }
}