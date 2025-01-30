﻿using Minio;

namespace MinioNotificationToRabbitMqApp.Factory
{
    internal class MinioClientFactory
    {
        static string endpoint = "localhost:9000";  // Используем 127.0.0.1, так как localhost может не работать правильно в Docker
        static string accessKey = "admin";
        static string secretKey = "admin123";
        static Lazy<IMinioClient> _instance = new Lazy<IMinioClient>(() =>
            new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithTimeout(5)
            .WithSSL(false)
            .Build()
        );

        public static IMinioClient GetClient()
        {
            return _instance.Value;
        }
    }
}
