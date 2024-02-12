using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class ProductsController(IProductsService service) : ControllerBase
    {
        private readonly IProductsService _service = service;

        [HttpGet]
        public async Task<IEnumerable<Product>> GetAll() => await _service.GetAll();

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Product>> Add(Product product)
        {
            await _service.Add(product);

            return CreatedAtAction(nameof(Add), product);
        }

        
        [HttpPut]
        [Authorize]
        public async Task<ActionResult> Update(Product product)
        {
            var prod = await _service.Update(product);

            return prod == null ? NotFound() : Ok(prod);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _service.Delete(id);

            return product == null ? NotFound() : Ok(product);  
        }
    }
}
