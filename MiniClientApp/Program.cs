using MiniClientApp;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Tags;
using Minio.Exceptions;
using System.Text;

namespace MinioClientApp
{
    // examples of code
    // https://min.io/docs/minio/linux/developers/dotnet/API.html
    internal class Program
    {
        //static string endpoint = "localhost:9000";  // Используем 127.0.0.1, так как localhost может не работать правильно в Docker
        //static string accessKey = "admin";
        //static string secretKey = "admin123";
        static string folder = @"C:\Users\Orkhan\Desktop\TestContainers";
        static string bucketName = "demo";
        static string fileName = "test.txt";
        static string downloadedFileName = "server-text.txt";
        static string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        static string dpath = Path.Combine(folder, downloadedFileName);

        static async Task EnableVersionAsync(IMinioClient minio,string bucketName)
        {
            try
            {
                var bargs = new BucketExistsArgs().WithBucket(bucketName);
                var isExist = await minio.BucketExistsAsync(bargs);
                
                if (isExist)
                {
                    Console.WriteLine($"Корзина {bucketName}  найдена ");
                    var varg = new SetVersioningArgs()
                        .WithVersioningEnabled()
                        .WithBucket(bucketName);
                    await minio.SetVersioningAsync(varg);
                }
                else
                {
                    Console.WriteLine("Создай корзину");
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine("Error occurred: " + e);
            }

        }
        static async Task DisableVersionAsync(IMinioClient minio, string bucketName)
        {
            try
            {
                var bargs = new BucketExistsArgs().WithBucket(bucketName);
                var isExist = await minio.BucketExistsAsync(bargs);

                if (isExist)
                {
                    Console.WriteLine($"Корзина {bucketName}  найдена ");
                    var varg = new SetVersioningArgs()
                        .WithVersioningEnabled()
                        .WithBucket(bucketName);
                    await minio.SetVersioningAsync(varg);
                }
                else
                {
                    Console.WriteLine("Создай корзину");
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine("Error occurred: " + e);
            }
        }
        static async Task Main(string[] args)
        {
            try
            {
                IMinioClient client = ClientFactory.GetClient();
                var content = Encoding.UTF8.GetBytes("Privet s bolshogo boduna");
                var path = Path.Combine("folder", "text.txt").Replace('\\','/');
                using Stream stream=new MemoryStream(content);
                PutObjectArgs pArg = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithContentType("text/plain")
                    .WithObject(path)
                    .WithObjectSize(content.Length)
                    .WithStreamData(stream);
                var put=await client.PutObjectAsync(pArg);
                Console.WriteLine(put.ResponseStatusCode);
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
        static async Task<string> GetContent()
        {
            if (File.Exists(path))
            {
                var text = await File.ReadAllTextAsync(path);
                return text;
            }
            else
            {
                return string.Empty;
            }
        }
        static void DeleteFile()
        {

            if (File.Exists(path))
                File.Delete(path);
        }
        static async Task PutFileInBucketAsync(IMinioClient minio)
        {
            try
            {
                if (!File.Exists(path))
                    throw new Exception("No any file to put");
                var putArg = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject($"files/{fileName}")
                    .WithFileName(path)
                    .WithContentType("text/plain");
                _ = await minio.PutObjectAsync(putArg);

                Console.WriteLine($"Uploaded object {fileName} to bucket {bucketName}");
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static async Task GetFileFromBucketAsync(IMinioClient minio)
        {
            try
            {


                if (minio == null)
                    throw new ArgumentNullException("Client is null");
                var args = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject($"files/{fileName}")
                    .WithFile(dpath);

                var f = await minio.GetObjectAsync(args);
                Console.WriteLine($"Downloaded the file {fileName} from bucket {bucketName}");
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static async Task GetFileFromBucketWithStreamAsync(IMinioClient minio)
        {
            try
            {

                if (minio == null)
                    throw new ArgumentNullException("Client is null");
                using var mstream = new MemoryStream();
                var args = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject($"files/{fileName}")
                    .WithCallbackStream(async (stream) => { await stream.CopyToAsync(mstream); });

                var f = await minio.GetObjectAsync(args);

                mstream.Position = 0;

                using StreamReader rstream = new StreamReader(mstream);
                var content = await rstream.ReadToEndAsync();
                Console.WriteLine($"Downloaded the file {fileName} from bucket {bucketName}");
                Console.WriteLine();
                Console.WriteLine($"Object content: {content}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static async Task PutFileInBucketWithStreamAsync(IMinioClient minio)
        {
            try
            {
                var text = Encoding.UTF8.GetBytes(GenerateLoremIpsumText(100));
                using var mstream = new MemoryStream(text);
                var putArg = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject($"files/{fileName}")
                    .WithStreamData(mstream)
                    .WithObjectSize(mstream.Length)
                    .WithContentType("text/plain");
                _ = await minio.PutObjectAsync(putArg);

                Console.WriteLine($"Uploaded object {fileName} to bucket {bucketName}");
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion
        #region Tags Up/Down load
        static public async Task UploadObjectWithStreamAndTags(IMinioClient minio,string version)
        {
            try
            {
                var text = GenerateLoremIpsumText(100);
                Dictionary<string, string> tagsDict = new Dictionary<string, string>
            {

                { "version",  version },// example version--> 1.0 
                { "content-type", "text/plain" },
                { "timestamp", DateTime.UtcNow.ToString() }
            };

                var tags = new Tagging(tagsDict, true);

                using MemoryStream msream = new MemoryStream(Encoding.UTF8.GetBytes(text));
                var args = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject($"files/{fileName}")
                    .WithStreamData(msream)
                    .WithObjectSize(msream.Length)
                    .WithContentType("text/plain")
                    .WithTagging(tags);

                await minio.PutObjectAsync(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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
