using System.Text.RegularExpressions;

namespace Api
{
    public static partial class FormFileExtensions
    {
        private const string REGEX_PATTERN = @"(.*\.)(jpe?g|png)$";
        private const RegexOptions REGEX_OPTIONS = RegexOptions.Multiline | RegexOptions.IgnoreCase;

        public async static Task<bool> SaveToDisk(this IFormFile file, string directoryPath)
        {
            var extension = Path.GetExtension(file.FileName);

            if (!ImageExtensionRegex().IsMatch(extension))
                return false;

            Directory.CreateDirectory(directoryPath);

            var fullPath = Path.Combine(directoryPath, file.FileName);

            using var stream = file.OpenReadStream();
            using var fileStream = File.Create(fullPath);
            await stream.CopyToAsync(fileStream);

            return true;
        }

        [GeneratedRegex(REGEX_PATTERN, REGEX_OPTIONS)]
        private static partial Regex ImageExtensionRegex();
    }
}
