using Api.Models;
using FluentFTP;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace Api.Services
{
    public class ProductItemService(IConfiguration configuration, IAsyncFtpClient ftpClient, AdminContext adminContext) : IProductItemService
    {
        private readonly AdminContext _adminContext = adminContext;
        private readonly IAsyncFtpClient _ftpClient = ftpClient;
        private readonly IConfiguration _configuration = configuration;

        public async Task<IEnumerable<ProductItem>> GetAll() => await _adminContext.ProductItems.ToListAsync();

        public async Task<ProductItem?> GetById(int id) => await _adminContext.ProductItems.FirstOrDefaultAsync(pi => pi.Id == id);

        public async Task<ProductItem?> Add(ProductItem productItem)
        {
            ArgumentNullException.ThrowIfNull(productItem, nameof(productItem));

            var file = productItem.File;

            if (file?.Length > 0 && await file.UploadToFtp(_ftpClient, _configuration["FTP:ImagePath"] ?? throw new InvalidConfigurationException("FTP image path is empty...")))
                productItem.ImageUrl = _configuration["FTP:Url"] + file.FileName;

            _adminContext.ProductItems.Add(productItem);
            await _adminContext.SaveChangesAsync();

            return productItem;
        }

        public async Task<ProductItem?> Update(ProductItem productItem)
        {
            ArgumentNullException.ThrowIfNull(productItem, nameof(productItem));

            var pi = await _adminContext.ProductItems.FirstOrDefaultAsync(pi => pi.Id == productItem.Id);

            if (pi == null) return null;

            var file = productItem.File;

            if (file?.Length > 0 && await file.UploadToFtp(_ftpClient, _configuration["FTP:ImagePath"] ?? throw new InvalidConfigurationException("FTP image path is empty...")))
                pi.ImageUrl = _configuration["FTP:Url"] + file.FileName;
            
            pi.Name = productItem.Name;
            pi.ProductSubTypeId = productItem.ProductSubTypeId;

            await _adminContext.SaveChangesAsync();

            return pi;
        }

        public async Task<ProductItem?> Delete(int id)
        {
            var pi = await _adminContext.ProductItems.FindAsync(id);

            if (pi == null) return null;

            _adminContext.ProductItems.Remove(pi);
            await _adminContext.SaveChangesAsync();

            return pi;
        }
    }
}
