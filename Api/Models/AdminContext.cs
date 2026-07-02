using Microsoft.EntityFrameworkCore;

namespace Api.Models
{
    public class AdminContext() : DbContext()
    {
        public DbSet<Product> Products { get; set;}
        public DbSet<ProductType> ProductTypes { get; set;}
        public DbSet<ProductSubType> ProductSubTypes { get; set;}
        public DbSet<ProductItem> ProductItems { get; set;}
        public DbSet<User> Users { get; set;}
        public DbSet<Item> Items { get; set;}
        public DbSet<ContactRequest> ContactRequests { get; set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Environment.GetEnvironmentVariable("SQLITE_DB_PATH") ?? "fulgur.db";
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}
