using Minio;
using Minio.DataModel.Args;
using MinioNotificationToRabbitMqApp.Factory;

namespace MinioNotificationToRabbitMqApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                IMinioClient client=ClientFactory.GetClient();
                var request = new MakeBucketArgs()
                    .WithBucket("demo");
                await client.MakeBucketAsync(request);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            



           
        }
    }
}
