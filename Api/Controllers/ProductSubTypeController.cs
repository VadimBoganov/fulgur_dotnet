using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductSubTypeController(IProductSubTypeService service) : ControllerBase
    {
        private readonly IProductSubTypeService _service = service;

        [HttpGet]
        public async Task<IEnumerable<ProductSubType>> GetAll() => await _service.GetAll();

        [HttpPost]
        public async Task<ActionResult<ProductSubType>> Add([FromBody] ProductSubType productSubType)
        {
            await _service.Add(productSubType);

            return CreatedAtAction(nameof(Add), productSubType);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, ProductSubType productSubType)
        {
            var pst = await _service.Update(id, productSubType);

            return pst == null ? NotFound() : Ok(pst);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var pst = await _service.Delete(id);

            return pst == null ? NotFound() : Ok(pst);
        }
    }
}
