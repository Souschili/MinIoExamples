using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using MinioApi.Services.Contracts;

namespace MinioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MinioController : ControllerBase
    {
        private readonly IMinioClient _client;
        private readonly string bucketName = "demo";
        private readonly string objectName = "vault";
        private readonly IRemoteFileService _fileService;


        public MinioController(IMinioClient client, IRemoteFileService fileService)
        {
            _client = client;
            _fileService = fileService;
        }

        [HttpGet("Download")]
        public async Task<IActionResult> Download()
        {
            try
            {
                
                var file= await _fileService.DownloadFileAsync("vault", "CV.pdf");
                var path = Path.Combine("C:/Test", "CV.pdf");
                // Открытие файла на запись
                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    // Копируем данные из memoryStream в файл
                    await file.CopyToAsync(fileStream);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> UploadAsync(IFormFile file)
        {
            try
            {
                var result = await _fileService.UploadFileAsync(file, objectName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadStreamAsync(IFormFile file)
        {
            try
            {
                using var sw = file.OpenReadStream();
                var result = await _fileService.UploadFileAsync(sw, "", objectName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Remove(string objectName,string fileName)
        {
            try
            {
                await _fileService.RemoveFileAsync(objectName,fileName);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Files")]
        public async Task<IActionResult> GetFiles()
        {
            try
            {
                string prefix = "vault/";
                var list=await _fileService.GetFilesListAsync(prefix);
                return Ok(list);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Info")]
        public async Task<IActionResult> GetInfo()
        {
            try
            {
                var info = await _fileService.GetFileInfoAsync("CV.pdf", "vault");
                return Ok(info);
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
