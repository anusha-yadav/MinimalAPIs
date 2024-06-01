using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryManagement.Data
{
    /// <summary>
    /// The Books
    /// </summary>
    public class Books
    {
        [Key]
        public int BookId { get; set; }


        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Publication Date is required")]
        public DateTime PublicationDate { get; set; }

        [Required(ErrorMessage = "ISBN is required")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Author is required")]
        public int AuthorId { get; set; }

        [JsonIgnore]
        [ForeignKey("AuthorId")]
        public virtual Author Author { get; set; }
    }
}
