using Api.Models;
using FluentFTP;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class ProductItemService(IConfiguration configuration, IAsyncFtpClient ftpClient, AdminContext adminContext) : IProductItemService
    {
        private readonly AdminContext _adminContext = adminContext;
        private readonly IAsyncFtpClient _ftpClient = ftpClient;
        private readonly IConfiguration _configuration = configuration;

        public async Task<IEnumerable<ProductItem>> GetAll() => await _adminContext.ProductItems.ToListAsync();

        public async Task<ProductItem?> GetById(int id) => await _adminContext.ProductItems.FirstOrDefaultAsync(pi => pi.Id == id);

        public async Task<ProductItem?> Add(ProductItem productItem, IFormFile file)
        {
            if (productItem == null)
                return null;

            var fullPath = $"{_configuration["FTP:ImagePath"]}{Guid.NewGuid()}.{Path.GetExtension(file.FileName)}";

            if (file.Length > 0)
            {
                var steam = file.OpenReadStream();
                await _ftpClient.UploadStream(steam, fullPath, FtpRemoteExists.Overwrite);
            }

            productItem.ImageUrl = fullPath;

            _adminContext.ProductItems.Add(productItem);
            await _adminContext.SaveChangesAsync();

            return productItem;
        }

        public async Task<ProductItem?> Update(ProductItem productItem, IFormFile file)
        {
            if (productItem == null)
                return null;

            var pi = await _adminContext.ProductItems.FirstOrDefaultAsync(pi => pi.Id == productItem.Id);

            if (pi == null)
                return null;

            var fullPath = $"{_configuration["FTP:ImagePath"]}{Guid.NewGuid()}.{Path.GetExtension(file.FileName)}";

            if (file.Length > 0)
            {
                var steam = file.OpenReadStream();
                await _ftpClient.UploadStream(steam, fullPath, FtpRemoteExists.Overwrite);
            }

            pi.ImageUrl = fullPath;
            pi.Name = productItem.Name;
            pi.ProductSubTypeId = productItem.ProductSubTypeId;

            await _adminContext.SaveChangesAsync();

            return pi;
        }

        public async Task<ProductItem?> Delete(int id)
        {
            var pi = await _adminContext.ProductItems.FindAsync(id);

            if (pi == null)
                return null;

            _adminContext.ProductItems.Remove(pi);
            await _adminContext.SaveChangesAsync();

            return pi;
        }
    }
}
