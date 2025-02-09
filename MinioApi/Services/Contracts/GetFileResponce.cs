using System.Collections.ObjectModel;

namespace MinioApi.Services.Contracts
{
    public class GetFileResponce
    {
        public GetFileResponce(ReadOnlyCollection<byte> _content)
        {
            this.Content = _content;
        }
        public IReadOnlyCollection<byte> Content { get; }
       
    }
}