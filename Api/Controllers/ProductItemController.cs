using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductItemController(IProductItemService service) : ControllerBase
    {
        private readonly IProductItemService _service = service;

        [HttpGet]
        public async Task<IEnumerable<ProductItem>> GetAll() => await _service.GetAll();

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductItem>> GetById(int id)
        {
            var pi = await _service.GetById(id);

            return pi == null ? NotFound() : Ok(pi);
        }

        [HttpPost]
        public async Task<ActionResult<ProductItem>> Add(ProductItem productItem, IFormFile file)
        {
            await _service.Add(productItem, file);

            return CreatedAtAction(nameof(Add), productItem);
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody]ProductItem productItem, IFormFile file)
        {
            var pi = await _service.Update(productItem, file);

            return pi == null ? NotFound() : Ok(pi);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var pi = await _service.Delete(id);

            return pi == null ? NotFound() : Ok(pi);
        }
    }
}
