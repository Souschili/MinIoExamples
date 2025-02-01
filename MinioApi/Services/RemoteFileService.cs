using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using MinioApi.Config;
using MinioApi.Services.Contracts;

namespace MinioApi.Services
{
    public class RemoteFileService : IRemoteFileService
    {
        private readonly RemoteFileConfig _config;
        private readonly IMinioClient _client;

        public RemoteFileService(IOptions<RemoteFileConfig> options,IMinioClient client)
        {
            if(options.Value == null)
                throw new ArgumentNullException(nameof(options.Value),"Config not loaded");
            if (client == null)
                throw new ArgumentNullException(nameof(client), "Client not initialized");
            _config = options.Value;
            _client = client;
        }

        public Task<PutObjectResponse> UploadFileAsync(IFormFile file, CancellationTokenSource ct)
        {
            throw new NotImplementedException();
        } 
        public async Task<bool> IsBucketExist()
        {
            BucketExistsArgs arg = new BucketExistsArgs().WithBucket(_config.BucketName);
            return await _client.BucketExistsAsync(arg);
        }
    }
}
