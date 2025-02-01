using Microsoft.Extensions.Options;
using Minio;
using MinioApi.Config;
using MinioApi.Services;
using MinioApi.Services.Contracts;

namespace MinioApi
{
    public static class ExtensionService
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            // create Minioclient
            AddMinio(services, configuration);

            // Add services to DI container
            AddServices(services);
          
        }

        public static void AddMinio(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RemoteFileConfig>(configuration.GetSection("RemoteFile"));

            services.AddSingleton<IMinioClient>(provider =>
            {
                var config = provider.GetRequiredService<IOptions<RemoteFileConfig>>().Value;
                var logger = provider.GetRequiredService<ILogger<IMinioClient>>();

                logger.LogInformation("Initializing MinIO Client with Endpoint: {Endpoint}, SSL: {UseSSL}", config.Host, config.UseSSL);

                return new MinioClient()
                    .WithCredentials(config.UserName, config.Password)
                    .WithEndpoint(config.Host)
                    .WithSSL(config.UseSSL)
                    .Build();
            });
        }
        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IRemoteFileService, RemoteFileService>();
        }
    }
}
