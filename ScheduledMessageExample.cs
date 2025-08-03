using Azure.Messaging.ServiceBus;
using ServiceBusDemo.Configuration;
using System.Text.Json;

namespace ServiceBusDemo
{
    public class ScheduledMessage
    {
        private readonly AppSettings settings;
        private readonly ServiceBusClient client;
        private readonly ServiceBusSender sender;

        public ScheduledMessage()
        {
            settings = ConfigurationHelper.LoadSettings();
            client = new ServiceBusClient(settings.ConnectionString);
            sender = client.CreateSender(settings.QueueName);
        }

        public async Task SendScheduledMessagesAsync()
        {
            Console.WriteLine("=== Scheduled Messages Demo ===");
            Console.WriteLine($"Connecting to queue: {settings.QueueName}");
            Console.WriteLine();

            try
            {
                await ScheduleSmallMessageAsync();                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scheduling messages: {ex.Message}");
            }
            finally
            {
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private async Task ScheduleSmallMessageAsync()
        {
            var currentTime = DateTimeOffset.UtcNow;
            var scheduleTime = currentTime.AddSeconds(30);
            
            Console.WriteLine($"Current time (UTC): {currentTime:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Scheduling message for: {scheduleTime:yyyy-MM-dd HH:mm:ss} UTC (30 seconds from now)");
            Console.WriteLine();
            
            var messageData = new
            {
                Id = Guid.NewGuid(),
                Text = "This is a scheduled message",
                ScheduledFor = scheduleTime,
                CurrentTime = currentTime,
                MessageSize = "Small (under 256KB)"
            };

            var messageBody = JsonSerializer.Serialize(messageData);
            var message = new ServiceBusMessage(messageBody)
            {
                MessageId = messageData.Id.ToString(),
                Subject = "ScheduledMessage",
                ContentType = "application/json"
            };

            message.ApplicationProperties.Add("MessageType", "Scheduled");
            message.ApplicationProperties.Add("Size", "Small");

            var sequenceNumber = await sender.ScheduleMessageAsync(message, scheduleTime);
            
            Console.WriteLine($"✓ Small message scheduled successfully");
            Console.WriteLine($"  - Message ID: {messageData.Id}");
            Console.WriteLine($"  - Sequence Number: {sequenceNumber}");
            Console.WriteLine($"  - Current time: {currentTime:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine($"  - Scheduled for: {scheduleTime:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine($"  - Time until delivery: 30 seconds");
            Console.WriteLine($"  - Message size: ~{messageBody.Length} bytes");
            Console.WriteLine();
            Console.WriteLine("The message will be delivered to the queue in 30 seconds.");
            Console.WriteLine("Start the receiver to see it being processed!");
            Console.WriteLine();
        }

        public async Task CancelScheduledMessageAsync(long sequenceNumber)
        {
            try
            {
                await sender.CancelScheduledMessageAsync(sequenceNumber);
                Console.WriteLine($"✓ Scheduled message with sequence number {sequenceNumber} was canceled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to cancel scheduled message: {ex.Message}");
            }
        }
    }
}
