namespace MinioApi.Services.Contracts
{
    public interface IMiniClient
    {
        Task<GetFileResponce> GetFileAsync(string fileNmae, string folderPath);
    }
}
