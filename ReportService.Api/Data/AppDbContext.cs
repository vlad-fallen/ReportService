using Microsoft.EntityFrameworkCore;
using ReportService.Api.Models;

namespace ReportService.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Item>().Property(i => i.Id).ValueGeneratedOnAdd();
        }

        public DbSet<Item> Items => Set<Item>();
    }
}
