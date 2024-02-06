using Api.Models;

namespace Api.Services
{
    public interface IProductTypesService
    {
        Task<ProductType> Add(ProductType productType);
        Task<ProductType?> Delete(int id);
        Task<IEnumerable<ProductType>> GetAll();
        Task<ProductType?> GetByProductId(int productId);
        Task<ProductType?> Update(int id, ProductType productType);
    }
}