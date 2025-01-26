using MiniClientApp;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Encryption;
using System;
using System.IO;
using System.Runtime.Intrinsics.X86;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace MinioClientApp
{
    internal class Program
    {
        //static string endpoint = "localhost:9000";  // Используем 127.0.0.1, так как localhost может не работать правильно в Docker
        //static string accessKey = "admin";
        //static string secretKey = "admin123";
        static string bucketName = "mydemo";
        static string fileName = "test.txt";


        static async Task Main(string[] args)
        {
            try
            {
                IMinioClient client = ClientFactory.GetClient();
                await CreateFile();
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static async Task<bool> IsBucketExistsAsync(IMinioClient minio)
        {
            var arg = new BucketExistsArgs()
                .WithBucket(bucketName);
            var isExists = await minio.BucketExistsAsync(arg);
            return isExists;
        }
        static async Task CreateBacketAsync(IMinioClient minio)
        {
            if (minio is null) throw new ArgumentNullException(nameof(minio));
            try
            {
                var arg = new BucketExistsArgs().WithBucket(bucketName);
                bool exists = await minio.BucketExistsAsync(arg);
                if (exists)
                {
                    Console.WriteLine($"Bucket with name {bucketName} is exist");
                    return;
                }


                await minio.MakeBucketAsync(
                    new MakeBucketArgs()
                        .WithBucket(bucketName)
                //.WithLocation(loc)
                ).ConfigureAwait(false);
                Console.WriteLine($"Created bucket {bucketName}");

            }
            catch (Exception e)
            {
                Console.WriteLine($"[Bucket]  Exception: {e}");
            }
        }
        static async Task DeleteBacketAsync(IMinioClient minio)
        {
            try
            {
                await minio.RemoveBucketAsync(
                    new RemoveBucketArgs()
                        .WithBucket(bucketName)
                ).ConfigureAwait(false);
                Console.WriteLine($"Removed the bucket {bucketName} successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Bucket]  Exception: {e}");
            }
        }
        static async Task GetListBucket(IMinioClient minio)
        {
            try
            {
                Console.WriteLine("Running example for API: ListBucketsAsync");
                var list = await minio.ListBucketsAsync().ConfigureAwait(false);
                foreach (var bucket in list.Buckets)
                    Console.WriteLine($"{bucket.Name} {bucket.CreationDateDateTime} {bucket.CreationDate}");
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Bucket]  Exception: {e}");
            }
        }
        #region UP/Down load as stream
        static async Task UploadDataToBasketAsync(IMinioClient minio)
        {
            // upload stream and save in bucket
            try
            {
                var content = "This is a river of my liife";
                using var mstream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                var args = new PutObjectArgs()
    .WithBucket(bucketName)               // Указывает имя корзины (bucket), в которую будет загружен объект (обязательный параметр).
    .WithObject("document/demo")          // Указывает ключ объекта, который будет использован для идентификации файла в корзине. Это может быть путь или уникальный идентификатор для объекта.
                                          //.WithFileName("demo.txt")            // Указывает имя локального файла, который будет загружен. Этот параметр используется для указания имени файла с локального диска. Если используется поток (Stream), то он может быть не нужен.
    .WithContentType("text/plain")       // Указывает тип контента объекта. Это MIME-тип (например, "text/plain", "application/json"), который помогает указать, как содержимое файла будет интерпретироваться.
    .WithStreamData(mstream)          // Указывает поток данных, который будет загружен в корзину. Здесь используется `MemoryStream` для передачи данных, загружаемых в объект.
    .WithObjectSize(mstream.Length);      // Указывает размер объекта в байтах. Для потоков данных, таких как `MemoryStream`, это важно, чтобы сервер MinIO знал, сколько данных загружать.
                                          //.WithPartSize(5 * 1024 * 1024)      // Указывает размер части при многократной загрузке больших объектов. Размер части по умолчанию равен 5 МБ. Используется для разбивки больших объектов на части при их загрузке.
                                          //.WithHeaders(new Dictionary<string, string>()) // Указывает произвольные заголовки, которые могут быть переданы вместе с объектом. Это может быть полезно для добавления дополнительной информации о загружаемом объекте.
                                          //.WithTagging("key1=value1&key2=value2") // Указывает теги, которые можно прикрепить к объекту. Теги обычно используются для метки или организации объектов в MinIO.
                                          //.WithSse(ServerSideEncryption.AwsKms)  // Включает серверное шифрование с использованием KMS (Key Management Service) для защиты данных во время их загрузки. Можно указать и другие схемы шифрования.
                                          //.WithRetention(RetentionPolicy.NonCurrentVersion)  // Устанавливает политику хранения объекта (например, для его архивации или защиты от удаления).
                                          //.WithLegalHold(true)               // Включает правовую блокировку для объекта, чтобы предотвратить его удаление или изменение на время.
                                          //.WithStorageClass(StorageClass.Standard) // Устанавливает класс хранения для объекта (например, стандартный, редуцированный или архивный).
                                          //.WithCallbackUrl("https://example.com/callback") // Указывает URL для получения уведомлений о завершении загрузки объекта.
                                          //.WithCallbackData("callback data")   // Указывает данные, которые будут отправлены при обратном вызове.
                                          //.WithCallbackBody("callback body")   // Указывает тело запроса, которое будет отправлено при обратном вызове.

                await minio.PutObjectAsync(args).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static async Task DownloadObjectFromBusketAsync(IMinioClient minio)
        {
            // return stream because we write object as stream not as file

            try
            {
                Console.WriteLine("Running example for API: GetObjectAsync");

                using var memoryStream = new MemoryStream();

                var args = new GetObjectArgs()
                    .WithBucket(bucketName) // Указывает корзину
                    .WithObject("document/demo") // Указывает ключ объекта
                                                 //.WithFile("demo") // если мы записали объект как массив байтов то поле необязательно
                    .WithCallbackStream((stream) => stream.CopyTo(memoryStream)); // Копируем данные объекта в поток

                // Выполняем запрос на получение объекта
                var r = await minio.GetObjectAsync(args).ConfigureAwait(false);

                // Устанавливаем позицию потока в начало для чтения
                memoryStream.Position = 0;

                // Читаем текстовые данные
                using var reader = new StreamReader(memoryStream);
                var content = await reader.ReadToEndAsync();

                Console.WriteLine($"Object content: {content}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while getting object: {e.Message}");
            }
        }
        #endregion
        #region Up/Down load 
        static string GenerateLoremIpsumText(int wordCount)
        {
            var words = new[]
            {
                "lorem", "ipsum", "dolor", "sit", "amet", "consectetur",
                "adipiscing", "elit", "sed", "do", "eiusmod", "tempor",
                "incididunt", "ut", "labore", "et", "dolore", "magna", "aliqua"
            };

            var random = new Random();
            return string.Join(" ", Enumerable.Range(0, wordCount)
                                  .Select(_ => words[random.Next(words.Length)]));
        }
        static async Task CreateFile()
        {
            try
            {
                string text = GenerateLoremIpsumText(100);
                string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                if (File.Exists(path))
                {
                    await File.AppendAllTextAsync(path, "This is added because file exists");
                    Console.WriteLine($"File with name {fileName} is exist");
                    return;

                }
                await File.WriteAllTextAsync(path, text);
                var message = GenerateLoremIpsumText(100);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static void DeleteFile()
        {

        }

        #endregion
        static async Task GetObjectListAsync(IMinioClient minio)
        {
            try
            {

                var listArgs = new ListObjectsArgs()
                    .WithBucket("myBucket")           // Указание корзины
                    .WithPrefix("documents/")         // Фильтрация объектов с префиксом "documents/"
                    .WithRecursive(true)              // Получить объекты во всех подкаталогах
                    .WithVersions(false)              // Не учитывать версии объектов
                    .WithIncludeUserMetadata(false);  // Не включать пользовательские метаданные



                await foreach (var item in minio.ListObjectsEnumAsync(listArgs).ConfigureAwait(false))
                    Console.WriteLine($"Object: {item.Key}");
                Console.WriteLine($"Listed all objects in bucket {bucketName}\n");
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }
    }
}
