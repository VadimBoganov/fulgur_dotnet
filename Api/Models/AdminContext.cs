using Microsoft.EntityFrameworkCore;

namespace Api.Models
{
    public class AdminContext() : DbContext()
    {
        public DbSet<Product> Products { get; set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=fulgur.db");
        }
    }
}
