using Api.Models;

namespace Api.Services
{
    public interface IItemService
    {
        Task<Item?> Add(Item item);
        Task<Item?> Delete(int id);
        Task<IEnumerable<Item>> GetAll();
        Task<IEnumerable<Item>> GetByProductItemId(int id);
        Task<Item?> Update(Item inputItem);
    }
}