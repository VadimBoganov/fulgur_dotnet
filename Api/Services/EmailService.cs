using Api.Models;
using System.Net;
using System.Net.Mail;

namespace Api.Services
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        public async Task SendContactRequestNotificationAsync(ContactRequest request)
        {
            var host = configuration["Smtp:Host"]!;
            var port = int.Parse(configuration["Smtp:Port"]!);
            var user = configuration["Smtp:User"]!;
            var password = configuration["Smtp:Password"]!;
            var to = configuration["Smtp:To"]!;

            var body = $"Name: {request.Name}\nEmail: {request.Email}\n" +
                       (request.Phone != null ? $"Phone: {request.Phone}\n" : "") +
                       $"Message: {request.Message}";

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(user, password)
            };

            using var message = new MailMessage(user, to, "New Message Received", body);
            await client.SendMailAsync(message);
        }
    }
}
