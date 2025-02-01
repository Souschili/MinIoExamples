using Minio.DataModel.Response;

namespace MinioApi.Services.Contracts
{
    public interface IRemoteFile
    {
        Task<PutObjectResponse> UploadFileAsync(IFormFile file, string objectName, CancellationToken ct = default);
        Task<PutObjectResponse> UploadFileAsync(Stream filestream, string fileName, string objectName, CancellationToken ct = default);
    }
}
