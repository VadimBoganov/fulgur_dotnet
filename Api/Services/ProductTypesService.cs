using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class ProductTypesService(AdminContext adminContext) : IProductTypesService
    {
        private readonly AdminContext _adminContext = adminContext;

        public async Task<IEnumerable<ProductType>> GetAll() => await _adminContext.ProductTypes.ToListAsync();

        public async Task<ProductType?> GetByProductId(int productId) => await _adminContext.ProductTypes.FirstOrDefaultAsync(pt => pt.ProductId == productId);

        public async Task<ProductType?> Add(ProductType productType)
        {
            if (productType == null)
                return null;

            _adminContext.ProductTypes.Add(productType);
            await _adminContext.SaveChangesAsync();

            return productType;
        }

        public async Task<ProductType?> Update(int id, ProductType productType)
        {
            if (productType == null)
                return null;

            var pt = await _adminContext.ProductTypes.FindAsync(id);

            if (pt == null)
                return null;

            pt.ProductId = productType.ProductId;
            pt.Name = productType.Name;

            try
            {
                await _adminContext.SaveChangesAsync();
            }
            catch (DbUpdateException) when (!_adminContext.ProductTypes.Any(pt => pt.Id == id))
            {
                return null;
            }

            return pt;
        }

        public async Task<ProductType?> Delete(int id)
        {
            var pt = await _adminContext.ProductTypes.FindAsync(id);

            if (pt == null)
                return null;

            _adminContext.ProductTypes.Remove(pt);
            await _adminContext.SaveChangesAsync();

            return pt;
        }
    }
}
