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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=fulgur.db");
        }
    }
}
