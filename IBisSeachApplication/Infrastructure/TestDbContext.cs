using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Infrastructure
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
           : base(options) { }
        public DbSet<Product>? Product { get; set; }
        public DbSet<SearchHistoryItem> SearchHistoryItem { get; set; }
        public DbSet<Users> Users { get; set; }
    }
}
