﻿using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class ProductSubTypeService(AdminContext adminContext) : IProductSubTypeService
    {
        private readonly AdminContext _adminContext = adminContext;

        public async Task<IEnumerable<ProductSubType>> GetAll() => await _adminContext.ProductSubTypes.ToListAsync();

        public async Task<ProductSubType?> Add(ProductSubType productSubType)
        {
            if (productSubType == null) return null;

            _adminContext.ProductSubTypes.Add(productSubType);
            await _adminContext.SaveChangesAsync();

            return productSubType;
        }

        public async Task<ProductSubType?> Update(ProductSubType productSubType)
        {
            if (productSubType == null) return null;

            var pst = await _adminContext.ProductSubTypes.FindAsync(productSubType.Id);

            if (pst == null) return null;

            pst.ProductTypeId = productSubType.ProductTypeId;
            pst.Name = productSubType.Name;

            await _adminContext.SaveChangesAsync();

            return pst;
        }

        public async Task<ProductSubType?> Delete(int id)
        {
            var pst = await _adminContext.ProductSubTypes.FindAsync(id);

            if (pst == null) return null;

            _adminContext.ProductSubTypes.Remove(pst);
            await _adminContext.SaveChangesAsync();

            return pst;
        }
    }
}
