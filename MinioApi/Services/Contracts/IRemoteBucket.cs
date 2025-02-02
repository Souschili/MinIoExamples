namespace MinioApi.Services.Contracts
{
    public interface IRemoteBucket
    {
        Task EnsureBucketExistsAsync(string bucketName);
    }
}
