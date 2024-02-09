using Api.Models;

namespace Api.Services
{
    public interface IItemService
    {
        Task<Item?> Add(Item item);
        Task<Item?> Delete(int id);
        Task<IEnumerable<Item>> GetAll();
        Task<Item?> GetById(int id);
        Task<Item?> Update(Item inputItem);
    }
}