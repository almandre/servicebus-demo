using Azure.Messaging.ServiceBus;
using ServiceBusDemo.Configuration;
using System.Text.Json;

namespace ServiceBusDemo
{
    public class Receiver
    {
        private readonly AppSettings settings;
        private readonly ServiceBusClient client;
        private readonly ServiceBusProcessor processor;

        public Receiver()
        {
            settings = ConfigurationHelper.LoadSettings();
            client = new ServiceBusClient(settings.ConnectionString);
            
            var options = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false,
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10)
            };
            
            processor = client.CreateProcessor(settings.QueueName, options);
        }

        public async Task ReceiveMessagesAsync()
        {
            Console.WriteLine("=== Azure Service Bus Receiver Demo ===");
            Console.WriteLine($"Connecting to queue: {settings.QueueName}");
            Console.WriteLine("Waiting for messages... (Press 'q' to exit)");
            Console.WriteLine();

            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            try
            {
                await processor.StartProcessingAsync();

                while (true)
                {
                    var key = Console.ReadKey(true);

                    if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                    {
                        Console.WriteLine("\nStopping processing...");

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during processing: {ex.Message}");
            }
            finally
            {
                await processor.StopProcessingAsync();
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }

            Console.WriteLine("\nReceiver finished.");
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            try
            {
                var body = args.Message.Body.ToString();
                var messageId = args.Message.MessageId;
                var subject = args.Message.Subject;
                var contentType = args.Message.ContentType;

                Console.WriteLine($"--- New Message Received ---");
                Console.WriteLine($"Message ID: {messageId}");
                Console.WriteLine($"Subject: {subject}");
                Console.WriteLine($"Content Type: {contentType}");
                Console.WriteLine($"Enqueued Time: {args.Message.EnqueuedTime:yyyy-MM-dd HH:mm:ss} UTC");
                Console.WriteLine($"Delivery Count: {args.Message.DeliveryCount}");

                if (args.Message.ApplicationProperties.Count > 0)
                {
                    Console.WriteLine("Custom Properties:");
                    foreach (var prop in args.Message.ApplicationProperties)
                    {
                        Console.WriteLine($"  {prop.Key}: {prop.Value}");
                    }
                }

                try
                {
                    var messageData = JsonSerializer.Deserialize<JsonElement>(body);
                    Console.WriteLine("Message Content:");
                    Console.WriteLine(JsonSerializer.Serialize(messageData, new JsonSerializerOptions 
                    { 
                        WriteIndented = true 
                    }));
                }
                catch
                {
                    Console.WriteLine($"Message Content (text): {body}");
                }

                Console.WriteLine("Processing message...");
                await Task.Delay(1000);

                await args.CompleteMessageAsync(args.Message);
                Console.WriteLine("✓ Message processed successfully");
                Console.WriteLine(new string('-', 50));
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing message {args.Message.MessageId}: {ex.Message}");
                
                // In case of error, you can decide to:
                // 1. Abandon the message (returns to queue)
                // await args.AbandonMessageAsync(args.Message);

                // 2. Send to dead letter queue
                // await args.DeadLetterMessageAsync(args.Message);

                // 3. For this example, we'll abandon
                await args.AbandonMessageAsync(args.Message);
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"❌ Processing error: {args.Exception.Message}");
            Console.WriteLine($"Source: {args.ErrorSource}");
            Console.WriteLine($"Namespace: {args.FullyQualifiedNamespace}");
            Console.WriteLine($"Entity Path: {args.EntityPath}");
            Console.WriteLine();

            return Task.CompletedTask;
        }

        public static async Task Main(string[] args)
        {
            var receiver = new Receiver();
            await receiver.ReceiveMessagesAsync();
        }
    }
}