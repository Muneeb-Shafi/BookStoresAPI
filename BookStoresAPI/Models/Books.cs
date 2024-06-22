using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class Books
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "text")]
        public string Title { get; set; }
        [Column(TypeName = "text")]
        public string Author { get; set; }
        [Column(TypeName = "text")]
        public string genre { get; set; }
    }
}
