using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookStoresAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "text")]
        public string Username { get; set; }
        [Column(TypeName = "text")]
        public string Email { get; set; }
        [Column(TypeName = "text")]
        public string Password { get; set; }
        [Column(TypeName = "text")]
        public string Role { get; set; }


    }
}
