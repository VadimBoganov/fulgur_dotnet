using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class ContactRequestsService(AdminContext adminContext, IEmailService emailService) : IContactRequestsService
    {
        private readonly AdminContext _adminContext = adminContext;
        private readonly IEmailService _emailService = emailService;

        public async Task<IEnumerable<ContactRequest>> GetAll() =>
            await _adminContext.ContactRequests.OrderByDescending(r => r.CreatedAt).ToListAsync();

        public async Task<ContactRequest?> Add(ContactRequest request)
        {
            if (request == null) return null;

            request.Status = ContactRequestStatus.New;
            request.CreatedAt = DateTime.UtcNow;

            _adminContext.ContactRequests.Add(request);
            await _adminContext.SaveChangesAsync();

            await _emailService.SendContactRequestNotificationAsync(request);

            return request;
        }

        public async Task<ContactRequest?> UpdateStatus(int id, ContactRequestStatus status)
        {
            var request = await _adminContext.ContactRequests.FindAsync(id);

            if (request == null) return null;

            request.Status = status;
            await _adminContext.SaveChangesAsync();

            return request;
        }

        public async Task<ContactRequest?> Delete(int id)
        {
            var request = await _adminContext.ContactRequests.FindAsync(id);

            if (request == null) return null;

            _adminContext.ContactRequests.Remove(request);
            await _adminContext.SaveChangesAsync();

            return request;
        }
    }
}