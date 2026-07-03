namespace Api
{
    public static class FormFileExtensions
    {
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; 

        private static readonly Dictionary<string, string[]> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = ["image/jpeg"],
            [".jpeg"] = ["image/jpeg"],
            [".png"] = ["image/png"],
        };

        /// <summary>
        /// Validates the uploaded file and stores it under a server-generated name.
        /// Returns the stored file name on success, or <c>null</c> if the file is rejected.
        /// </summary>
        public static async Task<string?> SaveToDisk(this IFormFile file, string directoryPath)
        {
            if (file.Length is <= 0 or > MaxFileSizeBytes)
                return null;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedTypes.TryGetValue(extension, out var allowedContentTypes))
                return null;

            if (!allowedContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
                return null;

            if (!await HasValidImageSignatureAsync(file, extension))
                return null;

            Directory.CreateDirectory(directoryPath);

            var storedName = $"{Guid.NewGuid():N}{extension}";
            var baseDir = Path.GetFullPath(directoryPath);
            var fullPath = Path.GetFullPath(Path.Combine(baseDir, storedName));

            if (!fullPath.StartsWith(baseDir + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                && fullPath != baseDir)
                return null;

            using var stream = file.OpenReadStream();
            using var fileStream = File.Create(fullPath);
            await stream.CopyToAsync(fileStream);

            return storedName;
        }

        // Verifies that the file's leading bytes match the declared image format,
        // so an attacker cannot smuggle HTML/scripts behind an image extension.
        private static async Task<bool> HasValidImageSignatureAsync(IFormFile file, string extension)
        {
            var header = new byte[8];

            await using var stream = file.OpenReadStream();
            var read = await stream.ReadAsync(header.AsMemory(0, header.Length));

            if (read < 4) return false;

            return extension switch
            {
                ".png" => header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47,
                ".jpg" or ".jpeg" => header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
                _ => false,
            };
        }
    }
}
