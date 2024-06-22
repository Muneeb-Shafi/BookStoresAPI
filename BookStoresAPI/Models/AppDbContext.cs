using BookStoresAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Books> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
