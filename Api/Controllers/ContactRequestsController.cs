using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactRequestsController(IContactRequestsService service) : ControllerBase
    {
        private readonly IContactRequestsService _service = service;

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<ContactRequest>> GetAll() => await _service.GetAll();

        [HttpPost]
        [EnableRateLimiting("contact")]
        public async Task<ActionResult<ContactRequest>> Add(ContactRequest request)
        {
            var result = await _service.Add(request);

            return CreatedAtAction(nameof(Add), result);
        }

        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] ContactRequestStatus status)
        {
            var request = await _service.UpdateStatus(id, status);

            return request == null ? NotFound() : Ok(request);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            var request = await _service.Delete(id);

            return request == null ? NotFound() : Ok(request);
        }
    }
}