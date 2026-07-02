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

            if (file?.Length > 0 && await file.SaveToDisk(_configuration["Images:StoragePath"] ?? throw new InvalidConfigurationException("Images storage path is empty...")))
            {
                item.ImageUrl = _configuration["Images:Url"] + file.FileName;

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

            if (file?.Length > 0 && await file.SaveToDisk(_configuration["Images:StoragePath"] ?? throw new InvalidConfigurationException("Images storage path is empty...")))
                item.ImageUrl = _configuration["Images:Url"] + file.FileName;

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

        public async Task<bool> BuldInsertAsync()
        {
            var productItems = _adminContext.ProductItems.AsQueryable().Where(pi => pi.Id > 19);
            
            foreach(var pi in productItems)
            {
                List<Item> items =
                [
                    new Item { ImageUrl = "https://gk-fulgur.ru/images/prutok2.jpg", IsFullPrice = true, Link = "#", Name = "Пруток", Price = 33.3f, ProductItemId = pi.Id  },
                    new Item { ImageUrl = "https://gk-fulgur.ru/images/lenta.jpg", IsFullPrice = true, Link = "#", Name = "Лента", Price = 33.3f, ProductItemId = pi.Id  },
                    new Item { ImageUrl = "https://gk-fulgur.ru/images/chushka.jpg", IsFullPrice = true, Link = "#", Name = "Чушка", Price = 33.3f, ProductItemId = pi.Id  },
                    new Item { ImageUrl = "https://gk-fulgur.ru/images/poroshok.jpg", IsFullPrice = true, Link = "#", Name = "Порошок", Price = 33.3f, ProductItemId = pi.Id  },
                    new Item { ImageUrl = "https://gk-fulgur.ru/images/trubka.jpg", IsFullPrice = true, Link = "#", Name = "Трубка", Price = 33.3f, ProductItemId = pi.Id  },
                    new Item { ImageUrl = "https://gk-fulgur.ru/images/slitok2.jpg", IsFullPrice = true, Link = "#", Name = "Слиток", Price = 33.3f, ProductItemId = pi.Id  },
                    new Item { ImageUrl = "https://gk-fulgur.ru/images/provoloka2.jpg", IsFullPrice = true, Link = "#", Name = "Проволока", Price = 33.3f, ProductItemId = pi.Id  },
                    new Item { ImageUrl = "https://gk-fulgur.ru/images/anod.jpg", IsFullPrice = true, Link = "#", Name = "Анод", Price = 33.3f, ProductItemId = pi.Id  },
                    new Item { ImageUrl = "https://gk-fulgur.ru/images/folga.jpg", IsFullPrice = true, Link = "#", Name = "Фольга", Price = 33.3f, ProductItemId = pi.Id  },
                ];

                var res = await BulkInsertByProductItemAsync(items);
                if (!res) {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> BulkInsertByProductItemAsync(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                await _adminContext.Items.AddAsync(item);
            }

            return await _adminContext.SaveChangesAsync() > 0;

        }
    }
}
