using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace MinioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MinioController : ControllerBase
    {
        private readonly IMinioClient _client;
        private readonly string bucketName = "demo";
        private readonly string objectName = "vault";

       
        public MinioController(IMinioClient client)
        {
            _client = client;
        }

        [HttpGet]
        public string Get() => "Test";

        [HttpPost("Upload")]
        public async Task<IActionResult> UploadAsync(IFormFile file)
        {
            try
            {
                if(file is null || file.Length == 0)
                {
                    return BadRequest("File can't be null or empty");
                }

                //проверяем баккет
                if(!await isBucketAsync())
                {
                    var buck=new MakeBucketArgs().WithBucket(bucketName);
                    await _client.MakeBucketAsync(buck);
                }
                // потом добавлю генератор 
                string objectPath = $"{objectName}/{file.FileName}";

                using var sw = file.OpenReadStream();

                var arg = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithContentType(file.ContentType)
                    .WithObjectSize(file.Length)
                    .WithStreamData(sw)
                    .WithObject($"{objectName}/{file.FileName}");
                    var responce=await _client.PutObjectAsync(arg);

                return Ok(responce);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [NonAction]
        private async Task<bool> isBucketAsync()
        {
            var arg = new BucketExistsArgs()
                .WithBucket(bucketName);

            return await _client.BucketExistsAsync(arg);
        }


    }
}
