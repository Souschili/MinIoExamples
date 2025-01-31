using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Notification;
using MinioNotificationToRabbitMqApp.Factory;

namespace MinioNotificationToRabbitMqApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                IMinioClient client = ClientFactory.GetClient();
               

                Console.WriteLine("Уведомления настроены успешно.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошло исключение:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
