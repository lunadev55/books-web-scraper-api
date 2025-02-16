using BooksWebScraperWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksWebScraperWebAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Book> Books { get; set; }
    }
}
