using Api.Models;

namespace Api.Services
{
    public interface IProductsService
    {
        Task<IEnumerable<Product>> GetAll();
        Task<Product?> Add(Product product);
        Task<Product?> Update(Product product);
        Task<Product?> Delete(int id);
    }
}