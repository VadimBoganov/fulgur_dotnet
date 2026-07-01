using Api.Models;

namespace Api.Services
{
    public interface IContactRequestsService
    {
        Task<IEnumerable<ContactRequest>> GetAll();
        Task<ContactRequest?> Add(ContactRequest request);
        Task<ContactRequest?> UpdateStatus(int id, ContactRequestStatus status);
        Task<ContactRequest?> Delete(int id);
    }
}