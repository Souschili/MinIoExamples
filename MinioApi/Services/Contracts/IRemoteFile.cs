using Minio.DataModel.Response;

namespace MinioApi.Services.Contracts
{
    public interface IRemoteFile
    {
        Task<PutObjectResponse> UploadFileAsync(IFormFile file, CancellationTokenSource ct);
    }
}
