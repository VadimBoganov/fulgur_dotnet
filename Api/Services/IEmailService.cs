using Api.Models;

namespace Api.Services
{
    public interface IEmailService
    {
        Task SendContactRequestNotificationAsync(ContactRequest request);
    }
}
