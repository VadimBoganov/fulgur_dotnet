﻿using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductTypesController(IProductTypesService service) : ControllerBase
    {
        private readonly IProductTypesService _service = service;

        [HttpGet]
        public async Task<IEnumerable<ProductType>> GetAll() => await _service.GetAll();

        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductType>> GetByProductId(int productId) 
        {
            var pt = await _service.GetByProductId(productId);

            return pt == null ? NotFound() : Ok(pt);   
        }

        [HttpPost]
        public async Task<ActionResult<ProductType>> Add([FromBody] ProductType productType)
        {
            await _service.Add(productType);

            return CreatedAtAction(nameof(Add), productType);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, ProductType productType)
        {
            var pt = await _service.Update(id, productType);

            return pt == null ? NotFound() : Ok(pt);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var pt = await _service.Delete(id);

            return pt == null ? NotFound() : Ok(pt);
        }
    }
}
