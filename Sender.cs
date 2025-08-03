using Azure.Messaging.ServiceBus;
using ServiceBusDemo.Configuration;
using System.Text.Json;

namespace ServiceBusDemo
{
    public class Sender
    {
        private readonly AppSettings _settings;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;

        public Sender()
        {
            _settings = ConfigurationHelper.LoadSettings();
            _client = new ServiceBusClient(_settings.ConnectionString);
            _sender = _client.CreateSender(_settings.QueueName);
        }

        public async Task SendMessagesAsync()
        {
            Console.WriteLine("=== Azure Service Bus Sender Demo ===");
            Console.WriteLine($"Connecting to queue: {_settings.QueueName}");
            Console.WriteLine();

            try
            {
                await SendSingleMessagesAsync();
                await SendBatchMessagesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending messages: {ex.Message}");
            }
            finally
            {
                await _sender.DisposeAsync();
                await _client.DisposeAsync();
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private async Task SendSingleMessagesAsync()
        {
            Console.WriteLine("--- Sending Individual Messages ---");

            for (int i = 1; i <= 3; i++)
            {
                var messageData = new
                {
                    Id = Guid.NewGuid(),
                    MessageNumber = i,
                    Text = $"Individual message #{i}",
                    Timestamp = DateTime.UtcNow,
                    Sender = "Sender Application"
                };

                var messageBody = JsonSerializer.Serialize(messageData);
                var message = new ServiceBusMessage(messageBody)
                {
                    MessageId = messageData.Id.ToString(),
                    Subject = "IndividualMessage",
                    ContentType = "application/json"
                };

                message.ApplicationProperties.Add("MessageType", "Individual");
                message.ApplicationProperties.Add("Priority", "Normal");

                await _sender.SendMessageAsync(message);
                Console.WriteLine($"✓ Message {i} sent - ID: {messageData.Id}");

                await Task.Delay(1000);
            }

            Console.WriteLine();
        }

        private async Task SendBatchMessagesAsync()
        {
            Console.WriteLine("--- Sending Batch Messages ---");

            using var messageBatch = await _sender.CreateMessageBatchAsync();

            for (int i = 1; i <= 5; i++)
            {
                var messageData = new
                {
                    Id = Guid.NewGuid(),
                    MessageNumber = i,
                    Text = $"Batch message #{i}",
                    Timestamp = DateTime.UtcNow,
                    Sender = "Sender Application - Batch"
                };

                var messageBody = JsonSerializer.Serialize(messageData);
                var message = new ServiceBusMessage(messageBody)
                {
                    MessageId = messageData.Id.ToString(),
                    Subject = "BatchMessage",
                    ContentType = "application/json"
                };

                message.ApplicationProperties.Add("MessageType", "Batch");
                message.ApplicationProperties.Add("BatchNumber", i.ToString());

                if (!messageBatch.TryAddMessage(message))
                {
                    Console.WriteLine($"⚠️ Could not add message {i} to batch");
                    break;
                }

                Console.WriteLine($"+ Message {i} added to batch - ID: {messageData.Id}");
            }

            if (messageBatch.Count > 0)
            {
                await _sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"✓ Batch sent with {messageBatch.Count} messages");
            }

            Console.WriteLine();
        }

        public static async Task Main(string[] args)
        {
            var sender = new Sender();
            await sender.SendMessagesAsync();
        }
    }
}