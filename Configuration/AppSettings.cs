namespace ServiceBusDemo.Configuration
{
    public class AppSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string QueueName { get; set; } = "demo-queue";
    }
};