using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using Minio.Exceptions;
using MinioApi.Config;
using MinioApi.Services.Contracts;

namespace MinioApi.Services
{
    public class RemoteFileService : IRemoteFileService
    {
        private readonly RemoteFileConfig _config;
        private readonly IMinioClient _client;
        private readonly ILogger _logger;

        public RemoteFileService(IOptions<RemoteFileConfig> options, IMinioClient client, ILogger logger)
        {
            if (options.Value == null)
                throw new ArgumentNullException(nameof(options.Value), "Config not loaded");
            if (client == null)
                throw new ArgumentNullException(nameof(client), "Client not initialized");
            _config = options.Value;
            _client = client;
            _logger = logger;
        }

        public async Task<PutObjectResponse> UploadFileAsync(IFormFile file, string objectName, CancellationToken ct = default)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentNullException(nameof(file), "Unable to upload an empty file");

                if (!await IsBucketExistAsync(_config.BucketName))
                {
                    _logger.LogError($"Bucket {_config.BucketName} does not exist.");
                    throw new BucketNotFoundException();
                }

                // читаем поток и используем юзинг, чтобы потом правильно его закрыть
                using var sw = file.OpenReadStream();
                sw.Position = 0; // на всякий случай
                string objectPath = $"{objectName}/{file.FileName}";

                // создаем запрос 
                PutObjectArgs args = new PutObjectArgs()
                    .WithBucket(_config.BucketName)
                    .WithContentType(file.ContentType)
                    .WithObject(objectPath)
                    .WithStreamData(sw)
                    .WithObjectSize(sw.Length);

                var response = await _client.PutObjectAsync(args, ct);

                return response;
            }
            catch (MinioException ex)
            {
                _logger.LogError($"MinIO error while uploading file '{file.FileName}': {ex.Message}");
                throw;  // Пробрасываем исключение дальше
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                throw;  // Пробрасываем исключение дальше
            }
        }

        public async Task<bool> IsBucketExistAsync(string bucketName)
        {
            BucketExistsArgs arg = new BucketExistsArgs().WithBucket(_config.BucketName);
            return await _client.BucketExistsAsync(arg);
        }

     
    }
}
