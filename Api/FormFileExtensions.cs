using FluentFTP;
using System.Text.RegularExpressions;

namespace Api
{
    public static partial class FormFileExtensions
    {
        private const string REGEX_PATTERN = @"(.*\.)(jpe?g|png)$";
        private const RegexOptions REGEX_OPTIONS = RegexOptions.Multiline | RegexOptions.IgnoreCase;

        public async static Task<bool> UploadToFtp(this IFormFile file, IAsyncFtpClient client, string path)
        {
            var extension = Path.GetExtension(file.FileName);

            if (!ImageExtensionRegex().IsMatch(extension)) 
                return false;

            var fullFtpPath = $"{path}{file.FileName}";

            var steam = file.OpenReadStream();
            var status = await client.UploadStream(steam, fullFtpPath, FtpRemoteExists.Overwrite);

            return status == FtpStatus.Success;
        }

        [GeneratedRegex(REGEX_PATTERN, REGEX_OPTIONS)]
        private static partial Regex ImageExtensionRegex();
    }
}
