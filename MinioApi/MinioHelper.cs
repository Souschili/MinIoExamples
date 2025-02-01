namespace MinioApi
{
    public static class MinioHelper
    {
        private static readonly Dictionary<string, string> _mimeTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
        {".jpg", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".png", "image/png"},
        {".txt", "text/plain"},
        {".html", "text/html"},
        // add MIME types if needed
        };

        public static string GetContentType(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("File name cannot be empty.", nameof(fileName));
            }

            var extension = Path.GetExtension(fileName);

            return _mimeTypes.TryGetValue(extension, out var mimeType) ? mimeType : "application/octet-stream";
        }
    }
}
