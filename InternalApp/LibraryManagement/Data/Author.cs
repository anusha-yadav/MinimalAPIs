using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryManagement.Data
{
    /// <summary>
    /// The Author
    /// </summary>
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string AuthorName { get; set; }

        [JsonIgnore]
        [InverseProperty("Author")]
        public virtual ICollection<Books> Books { get; set; } = new HashSet<Books>();
    }
}
