using Microsoft.EntityFrameworkCore;
using PizzaSales.Models;

namespace PizzaSales.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Sales> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //
        }
    }
}
