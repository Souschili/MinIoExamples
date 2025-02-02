namespace MinioApi
{
    public static class MinioHelper
    {
       static Dictionary<string,string> _mimeTypes = new Dictionary<string, string>
        {
            {".3gp", "video/3gpp"},
            {".3g2", "video/3gpp2"},
            {".aac", "audio/aac"},
            {".abw", "application/x-abiword"},
            {".arc", "application/octet-stream"},
            {".avi", "video/x-msvideo"},
            {".azw", "application/vnd.amazon.ebook"},
            {".bin", "application/octet-stream"},
            {".bmp", "image/bmp"},
            {".bz", "application/x-bzip"},
            {".bz2", "application/x-bzip2"},
            {".c", "text/x-c"},
            {".cab", "application/vnd.ms-cab-compressed"},
            {".cc", "text/x-c++src"},
            {".css", "text/css"},
            {".csv", "text/csv"},
            {".doc", "application/msword"},
            {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {".eot", "application/vnd.ms-fontobject"},
            {".epub", "application/epub+zip"},
            {".gif", "image/gif"},
            {".gz", "application/gzip"},
            {".htm", "text/html"},
            {".html", "text/html"},
            {".ico", "image/x-icon"},
            {".ics", "text/calendar"},
            {".jar", "application/java-archive"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".js", "application/javascript"},
            {".json", "application/json"},
            {".jsonld", "application/ld+json"},
            {".mid", "audio/midi"},
            {".midi", "audio/midi"},
            {".mp3", "audio/mpeg"},
            {".mp4", "video/mp4"},
            {".mpeg", "video/mpeg"},
            {".mpg", "video/mpeg"},
            {".odp", "application/vnd.oasis.opendocument.presentation"},
            {".ods", "application/vnd.oasis.opendocument.spreadsheet"},
            {".odt", "application/vnd.oasis.opendocument.text"},
            {".ogg", "audio/ogg"},
            {".otf", "font/otf"},
            {".pdf", "application/pdf"},
            {".php", "application/x-httpd-php"},
            {".png", "image/png"},
            {".ppt", "application/vnd.ms-powerpoint"},
            {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
            {".rar", "application/x-rar-compressed"},
            {".rtf", "application/rtf"},
            {".sh", "application/x-sh"},
            {".svg", "image/svg+xml"},
            {".swf", "application/x-shockwave-flash"},
            {".tar", "application/x-tar"},
            {".tiff", "image/tiff"},
            {".torrent", "application/x-bittorrent"},
            {".txt", "text/plain"},
            {".wav", "audio/wav"},
            {".webm", "video/webm"},
            {".webp", "image/webp"},
            {".woff", "font/woff"},
            {".woff2", "font/woff2"},
            {".xls", "application/vnd.ms-excel"},
            {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {".xml", "application/xml"},
            {".xul", "application/vnd.mozilla.xul+xml"},
            {".zip", "application/zip"},
            {".7z", "application/x-7z-compressed"},
            {".m4v", "video/x-m4v"},
            {".opus", "audio/opus"}
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

        public static string GenerateObjectPath(string objectName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
                throw new ArgumentException("Object name cannot be empty", nameof(objectName));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be empty", nameof(fileName));

            return $"{objectName.TrimEnd('/')}/{fileName}";
        }

        public static string GeneratePrefix(string objectNamePrefix)
        {
            if (string.IsNullOrWhiteSpace(objectNamePrefix))
                throw new ArgumentException("Object name prefix cannot be empty.");

            if (!objectNamePrefix.EndsWith("/"))
            {
                objectNamePrefix += "/";
            }

            return objectNamePrefix;
        }

    }
}
