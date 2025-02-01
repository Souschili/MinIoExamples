namespace MinioApi.Services.Contracts
{
    public interface IRemoteBucket
    {
        Task<bool> IsBucketExistAsync(string bucketName);
    }
}
