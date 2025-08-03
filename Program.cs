using ServiceBusDemo;

Console.WriteLine("=== Azure Service Bus Demo ===");
Console.WriteLine();
Console.WriteLine("Choose an option:");
Console.WriteLine("1 - Run Sender (Send messages)");
Console.WriteLine("2 - Run Receiver (Receive messages)");
Console.WriteLine("0 - Exit");
Console.WriteLine();
Console.Write("Enter your choice: ");

var option = Console.ReadLine();

switch (option)
{
    case "1":
        Console.Clear();
        var sender = new Sender();
        await sender.SendMessagesAsync();
        break;
        
    case "2":
        Console.Clear();
        var receiver = new Receiver();
        await receiver.ReceiveMessagesAsync();
        break;
        
    case "0":
        Console.WriteLine("Exiting...");
        break;
        
    default:
        Console.WriteLine("Invalid option!");
        break;
}
