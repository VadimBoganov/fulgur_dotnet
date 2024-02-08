using Api.Models;
using FluentFTP;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class ProductItemService(IConfiguration configuration, ILogger<ProductItemService> logger, IAsyncFtpClient ftpClient, AdminContext adminContext) : IProductItemService
    {
        private readonly AdminContext _adminContext = adminContext;
        private readonly ILogger<ProductItemService> _logger = logger;
        private readonly IAsyncFtpClient _ftpClient = ftpClient;
        private readonly IConfiguration _configuration = configuration;

        public async Task<IEnumerable<ProductItem>> GetAll() => await _adminContext.ProductItems.ToListAsync();

        public async Task<ProductItem?> GetById(int id) => await _adminContext.ProductItems.FirstOrDefaultAsync(pi => pi.Id == id);

        public async Task<ProductItem?> Add(ProductItem productItem)
        {
            if (productItem == null) return null;

            var file = productItem.File;
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = $"{_configuration["FTP:ImagePath"]}{fileName}";

            if (file.Length > 0)
            {
                var steam = file.OpenReadStream();
                var status = await _ftpClient.UploadStream(steam, fullPath, FtpRemoteExists.Overwrite);

                if (status != FtpStatus.Success)
                {
                    _logger.LogError("Can't upload file to ftp server with status - {status}", status);
                    return null;
                }
            }

            productItem.ImageUrl = _configuration["FTP:Url"] + fileName;

            _adminContext.ProductItems.Add(productItem);
            await _adminContext.SaveChangesAsync();

            return productItem;
        }

        public async Task<ProductItem?> Update(ProductItem productItem)
        {
            if (productItem == null) return null;

            var pi = await _adminContext.ProductItems.FirstOrDefaultAsync(pi => pi.Id == productItem.Id);

            if (pi == null) return null;

            var file = productItem.File;
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = $"{_configuration["FTP:ImagePath"]}{fileName}";

            if (file.Length > 0)
            {
                var steam = file.OpenReadStream();
                var status = await _ftpClient.UploadStream(steam, fullPath, FtpRemoteExists.Overwrite);

                if (status != FtpStatus.Success)
                {
                    _logger.LogError("Can't upload file to ftp server with status - {status}", status);
                    return null;
                }
            }

            pi.ImageUrl = _configuration["FTP:Url"] + fileName;
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
