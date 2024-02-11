using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class ProductsService(AdminContext adminContext) : IProductsService
    {
        private readonly AdminContext _adminContext = adminContext;

        public async Task<IEnumerable<Product>> GetAll() => await _adminContext.Products.ToListAsync();

        public async Task<Product?> Add(Product product)
        {
            if (product == null) return null;

            _adminContext.Products.Add(product);
            await _adminContext.SaveChangesAsync();

            return product;
        }

        public async Task<Product?> Update(Product product)
        {
            if (product == null) return null;

            var prod = await _adminContext.Products.FindAsync(product.Id);

            if (prod == null) return null;

            prod.Name = product.Name;

            await _adminContext.SaveChangesAsync();

            return prod;
        }

        public async Task<Product?> Delete(int id)
        {
            var product = await _adminContext.Products.FindAsync(id);

            if (product == null) return null;

            _adminContext.Products.Remove(product);
            await _adminContext.SaveChangesAsync();

            return product;
        }
    }
}
