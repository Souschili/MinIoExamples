using Microsoft.Extensions.Options;
using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using Minio.Exceptions;
using MinioApi.Config;
using MinioApi.Services.Contracts;
using System.IO;
using System.Reactive.Linq;

namespace MinioApi.Services
{
    public class RemoteFileService : IRemoteFileService
    {
        private readonly RemoteFileConfig _config;
        private readonly IMinioClient _client;
        private readonly ILogger<RemoteFileService> _logger;

        public RemoteFileService(IOptions<RemoteFileConfig> options, IMinioClient client, ILogger<RemoteFileService> logger)
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

                await MinioHelper.EnsureBucketExistsAsync(_client, _config.BucketName, _logger);

                // читаем поток и используем юзинг, чтобы потом правильно его закрыть
                using var sw = file.OpenReadStream();
                sw.Position = 0; // на всякий случай

                string objectPath = MinioHelper.GenerateObjectPath(objectName, file.FileName);

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

        public async Task<PutObjectResponse> UploadFileAsync(Stream filestream, string fileName, string objectName, CancellationToken ct = default)
        {
            try
            {
                // Validate file stream, file name, and object name
                if (filestream == null || filestream.Length == 0)
                    throw new FileNotFoundException("File not found or empty.");

                await MinioHelper.EnsureBucketExistsAsync(_client, _config.BucketName,_logger);

                string objectPath = MinioHelper.GenerateObjectPath(objectName,fileName);
                filestream.Position = 0;

                string contentType = MinioHelper.GetContentType(fileName);

                PutObjectArgs args = new PutObjectArgs()
                    .WithBucket(_config.BucketName)
                    .WithContentType(contentType)
                    .WithObject(objectPath)
                    .WithStreamData(filestream)
                    .WithObjectSize(filestream.Length);

                var response = await _client.PutObjectAsync(args, ct);

                return response;
            }
            catch (MinioException ex)
            {
                _logger.LogError(ex, $"MinIO error while uploading file '{fileName}': {ex.Message}");
                throw;  // Rethrow the exception
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while uploading file '{fileName}': {ex.Message}");
                throw;  // Rethrow the exception
            }
        }

        public async Task RemoveFileAsync(string objectName, string fileName, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(objectName))
                    throw new ArgumentException("File name and object name cannot be empty.");

                var arg = new RemoveObjectArgs()
                                .WithBucket(_config.BucketName)
                                .WithObject($"{objectName}/{fileName}");
                await _client.RemoveObjectAsync(arg, ct);
            }
            catch (MinioException ex)
            {
                _logger.LogError(ex, $"MinIO error while remove file '{fileName}': {ex.Message}");
                throw;  // Rethrow the exception
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while remove file '{fileName}': {ex.Message}");
                throw;  // Rethrow the exception
            }
        }

       
        
        
        
        public async Task<Stream> DownloadFileAsync(string objectName, string fileName, CancellationToken ct = default)
        {
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(objectName))
                    throw new ArgumentException("File name and object name cannot be empty.");

                var args = new GetObjectArgs()
                    .WithBucket(_config.BucketName)
                    .WithObject($"{objectName}/{fileName}")
                    .WithCallbackStream(async (stream) =>
                    {
                        await stream.CopyToAsync(memoryStream, ct);
                        memoryStream.Position = 0;
                    });

                await _client.GetObjectAsync(args, ct);

                return memoryStream;
            }
            catch (MinioException ex)
            {
                _logger.LogError(ex, $"MinIO error while downloading file '{fileName}': {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while downloading file '{fileName}': {ex.Message}");
                throw;
            }
            finally
            {
                memoryStream?.Dispose();
            }
        }

        public async Task<ObjectStat> GetFileInfoAsync(string fileName, string objectName, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(objectName))
                    throw new ArgumentException("File name and object name cannot be empty.");

                var statObjectArgs = new StatObjectArgs()
                    .WithBucket(_config.BucketName)
                    .WithObject(objectName);

                var fileInfo = await _client.StatObjectAsync(statObjectArgs, ct);
                return fileInfo;
            }
            catch (MinioException ex)
            {
                //_logger.LogError(ex, $"MinIO error while listing files with prefix '{objectNamePrefix}': {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"Unexpected error while listing files with prefix '{objectNamePrefix}': {ex.Message}");
                throw;
            }
        }
        public async Task<List<Item>> GetFilesListAsync(string objectNamePrefix, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(objectNamePrefix))
                    throw new ArgumentException("Object name prefix cannot be empty.");

                var args = new ListObjectsArgs()
                    .WithBucket(_config.BucketName)
                    .WithPrefix(objectNamePrefix)
                    .WithRecursive(false);  // true - чтобы пройти по всем подкаталогам, false - только по текущей папке

                List<Item> list = new();
                await foreach (var item in _client.ListObjectsEnumAsync(args, ct))
                {
                    list.Add(item);
                }
                return list;
            }
            catch (MinioException ex)
            {
                _logger.LogError(ex, $"MinIO error while listing files with prefix '{objectNamePrefix}': {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while listing files with prefix '{objectNamePrefix}': {ex.Message}");
                throw;
            }
        }
        public async Task<bool> HasBucketAsync(string bucketName)
        {
            BucketExistsArgs arg = new BucketExistsArgs().WithBucket(_config.BucketName);
            return await _client.BucketExistsAsync(arg);
        }

        public Task<bool> HasFileAsync(string objectName, string fileName, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
