using Api.Models;

namespace Api.Services
{
    public interface IProductSubTypeService
    {
        Task<ProductSubType?> Add(ProductSubType productSubType);
        Task<ProductSubType?> Delete(int id);
        Task<IEnumerable<ProductSubType>> GetAll();
        Task<ProductSubType?> Update(int id, ProductSubType productSubType);
    }
}