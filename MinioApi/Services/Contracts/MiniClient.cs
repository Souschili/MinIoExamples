using System.Collections.ObjectModel;

namespace MinioApi.Services.Contracts
{
    public class MiniClient : IMiniClient
    {
        private readonly IRemoteFileService _remoteFileService;
       

        public MiniClient(IRemoteFileService remoteFileService)
        {
           
            _remoteFileService = remoteFileService;
        }

        public async Task<GetFileResponce> GetFileAsync(string fileNmae, string folderPath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(nameof(fileNmae));
           var response= await _remoteFileService.DownloadFileAsync(fileNmae, folderPath);
            byte[] buffer;
            using (MemoryStream ms = new MemoryStream())
            {
                response.CopyTo(ms);
                buffer = ms.ToArray();
            }
            return new GetFileResponce(new ReadOnlyCollection<byte>(buffer));
        }
    }
}
