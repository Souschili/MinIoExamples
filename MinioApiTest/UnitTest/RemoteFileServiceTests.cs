using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using Minio.Exceptions;
using MinioApi;
using MinioApi.Config;
using MinioApi.Services;
using Moq;
using System.Net;


namespace MinioApiTest.UnitTest
{

    public class RemoteFileServiceTests
    {
        private readonly Mock<IMinioClient> _mockMinioClient;
        private readonly Mock<ILogger<RemoteFileService>> _mockLogger;
        private readonly RemoteFileService _fileService;
        private readonly Mock<IOptions<RemoteFileConfig>> _mockOptions;

        public RemoteFileServiceTests()
        {
            _mockMinioClient = new Mock<IMinioClient>();
            _mockLogger = new Mock<ILogger<RemoteFileService>>();
            _mockOptions = new Mock<IOptions<RemoteFileConfig>>();

            _mockOptions.Setup(o => o.Value).Returns(new RemoteFileConfig { BucketName = "test-bucket" });

            _fileService = new RemoteFileService(_mockOptions.Object, _mockMinioClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task UploadFileAsync_Should_Throw_Exception_When_File_Is_Empty()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);  // Пустой файл

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _fileService.UploadFileAsync(fileMock.Object, "objectName"));
            Assert.Equal("Unable to upload an empty file (Parameter 'file')", exception.Message);
        }

        [Fact]
        public async Task EnsureBucketExistsAsync_BucketDoesNotExist_ThrowsBucketNotFoundException()
        {
            // Arrange
            var bucketName = "test-bucket";
            _mockMinioClient.Setup(client => client.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<BucketNotFoundException>(() => _fileService.EnsureBucketExistsAsync(bucketName));
        }

        [Fact]
        public async Task EnsureBucketExistsAsync_BucketExists_DoesNotThrowException()
        {
            // Arrange
            var bucketName = "test-bucket";
            _mockMinioClient.Setup(client => client.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync(true);

            // Act
            var exception = await Record.ExceptionAsync(() => _fileService.EnsureBucketExistsAsync(bucketName));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task UploadFileAsync_Should_Upload_File()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "Hello, Minio!";
            var fileName = "test.txt";
            var objectName = "test-object";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();  // Обязательно нужно сбросить данные в поток

            ms.Position = 0;  // Устанавливаем позицию потока в начало

            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.ContentType).Returns("text/plain");

            var bucketName = "test-bucket";
            _mockMinioClient.Setup(client => client.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync(true);

            _mockMinioClient.Setup(c => c.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(new PutObjectResponse(HttpStatusCode.OK, "etag", new Dictionary<string, string>(), ms.Length, objectName));

            // Act
            var result = await _fileService.UploadFileAsync(fileMock.Object, objectName);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UploadFileAsync_Showld_ThrowException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _fileService.UploadFileAsync(default!, "ggg"));
        }
    }
}
