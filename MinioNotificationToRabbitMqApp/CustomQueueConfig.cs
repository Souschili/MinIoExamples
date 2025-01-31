using Minio.DataModel.Notification;

namespace MinioNotificationToRabbitMqApp
{
    internal class CustomQueueConfig : QueueConfig
    {
        public string Queue { get; set; }
        public List<EventType> Events { get; set; }
        public string QueueAddress { get; set; }
    }
}