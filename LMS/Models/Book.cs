using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        [StringLength(13)]
        public string ISBN { get; set; }

        public int PublicationYear { get; set; }

        public int TotalCopies { get; set; }

        public int AvailableCopies { get; set; }

        [Required]
        public int AuthorId { get; set; }

        public Author? Author { get; set; }   // FIX HERE

        public ICollection<BorrowRecord>? BorrowRecords { get; set; }
    }
}