using FluentFTP;

namespace Api
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddFtpClient(this IServiceCollection services, IConfiguration configuration)
        {
            var ftpClient = new AsyncFtpClient
            {
                Host = configuration["FTP:Host"],
                Credentials = new System.Net.NetworkCredential
                {
                    UserName = configuration["FTP:Username"],
                    Password = configuration["FTP:Password"]
                }
            };

            services.AddSingleton<IAsyncFtpClient>(ftpClient);

            return services;
        }
    }
}
