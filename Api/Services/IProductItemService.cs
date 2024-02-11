using Api.Models;

namespace Api.Services
{
    public interface IProductItemService
    {
        Task<ProductItem?> Add(ProductItem productItem);
        Task<ProductItem?> Delete(int id);
        Task<IEnumerable<ProductItem>> GetAll();
        Task<ProductItem?> GetById(int id);
        Task<ProductItem?> Update(ProductItem productItem);
    }
}