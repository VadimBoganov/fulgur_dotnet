using FluentFTP;

namespace Api
{
    public static class FormFileExtensions
    {
        public async static Task<bool> UploadToFtp(this IFormFile file, IAsyncFtpClient client, string path)
        {
            var fullFtpPath = $"{path}{file.FileName}";

            var steam = file.OpenReadStream();
            var status = await client.UploadStream(steam, fullFtpPath, FtpRemoteExists.Overwrite);

            return status == FtpStatus.Success;
        }
    }
}
