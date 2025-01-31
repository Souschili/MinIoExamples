using Microsoft.AspNetCore.Mvc;
using Minio;

namespace MinioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MinioController : ControllerBase
    {
        private readonly IMinioClient _client;

        public MinioController(IMinioClient client)
        {
            _client = client;           
        }

        [HttpGet]
        public string Get() => "Test";
    }
}
