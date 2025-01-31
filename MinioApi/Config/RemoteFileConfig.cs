namespace MinioApi.Config
{
    public sealed class RemoteFileConfig
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; }= string.Empty;
        public string Host { get; set; } = string.Empty;
        public string Port { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public bool UseSSL { get; set; }
    }
}
