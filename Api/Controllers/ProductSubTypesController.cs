using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductSubTypesController(IProductSubTypeService service) : ControllerBase
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

        [HttpPut]
        public async Task<ActionResult> Update(ProductSubType productSubType)
        {
            var pst = await _service.Update(productSubType);

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
