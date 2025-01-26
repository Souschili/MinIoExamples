using Docker.DotNet.Models;
using Testcontainers.Minio;

namespace MiniioTests.TestFixture
{
    [CollectionDefinition("Minio collection")]
    public class MinioCollection : ICollectionFixture<MinioFixture>
    {
        // no code here
    }

    public sealed class MinioFixture : IAsyncLifetime
    {
        public MinioContainer Container { get; private set; } = default!;

        public async Task DisposeAsync()
        {
            await Container.DisposeAsync();
        }

        public async Task InitializeAsync()
        {
            string localPath = "/path/to/local/data";  // Путь к локальной папке на хосте
            string containerPath = "/data";            // Путь внутри контейнера

            Container = new MinioBuilder()
  .WithImage("minio/minio:RELEASE.2023-01-31T02-24-19Z")
  .Build();

            // Запускаем контейнер
            await Container.StartAsync();

        }
    }
}
