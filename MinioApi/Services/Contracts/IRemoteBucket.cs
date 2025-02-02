namespace MinioApi.Services.Contracts
{
    public interface IRemoteBucket
    {
        Task<bool> HasBucketAsync(string bucketName);
    }
}
