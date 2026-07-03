using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.Configuration;
using System.ComponentModel;

namespace Api.Services
{
    public class ItemService(IConfiguration configuration, ILogger<ItemService> logger, AdminContext adminContext) : IItemService
    {
        private readonly AdminContext _adminContext = adminContext;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<ItemService> _logger = logger;

        public async Task<IEnumerable<Item>> GetAll() => await _adminContext.Items.ToListAsync();

        public async Task<IEnumerable<Item>> GetByProductItemId(int id) => await _adminContext.Items.Where(i => i.ProductItemId == id).ToListAsync();

        public async Task<Item?> Add(Item item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));

            var file = item.File;

            var storedName = file?.Length > 0
                ? await file.SaveToDisk(_configuration["Images:StoragePath"] ?? throw new InvalidConfigurationException("Images storage path is empty..."))
                : null;

            if (storedName != null)
            {
                item.ImageUrl = _configuration["Images:Url"] + storedName;

                _adminContext.Items.Add(item);
                await _adminContext.SaveChangesAsync();

                return item;
            }
            else throw new Exception($"Upload file {file?.FileName} is falied...");
        }

        public async Task<Item?> Update(Item inputItem)
        {
            ArgumentNullException.ThrowIfNull(inputItem, nameof(inputItem));

            var item = await _adminContext.Items.FindAsync(inputItem.Id);

            if (item == null) return null;

            var file = inputItem.File;

            var storedName = file?.Length > 0
                ? await file.SaveToDisk(_configuration["Images:StoragePath"] ?? throw new InvalidConfigurationException("Images storage path is empty..."))
                : null;

            if (storedName != null)
                item.ImageUrl = _configuration["Images:Url"] + storedName;

            item.Name = inputItem.Name;
            item.IsFullPrice = inputItem.IsFullPrice;
            item.ProductItemId = inputItem.ProductItemId;
            item.Price = inputItem.Price;
            item.Link = inputItem.Link;

            await _adminContext.SaveChangesAsync();

            return item;
        }

        public async Task<Item?> Delete(int id)
        {
            var item = await _adminContext.Items.FindAsync(id);

            if (item == null) return null;

            _adminContext.Items.Remove(item);
            await _adminContext.SaveChangesAsync();

            return item;
        }
    }
}
