using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController(IItemService service) : ControllerBase
    {
        private readonly IItemService _service = service;

        [HttpGet]
        public async Task<IEnumerable<Item>> GetAll() => await _service.GetAll();

        [HttpGet("{productItemId}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetByProductItemId(int productItemId) =>
            await _service.GetByProductItemId(productItemId) == null ? NotFound() : Ok();

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Item>> Add([FromForm] Item item) => CreatedAtAction(nameof(Add), await _service.Add(item));

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> Update([FromForm] Item inputItem)
        {
            var item = await _service.Update(inputItem);

            return item == null ? NotFound() : Ok(item);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            var item = await _service.Delete(id);

            return item == null ? NotFound() : Ok(item);
        }
    }
}
