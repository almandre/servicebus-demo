# Azure Service Bus Demo

This is a demonstration application for Azure Service Bus that shows how to send and receive messages using queues.

## Project Structure

- **Program.cs** - Main menu to choose between Sender and Receiver
- **Sender.cs** - Application that sends messages to the Service Bus queue
- **Receiver.cs** - Application that receives and processes messages from the queue
- **Configuration/** - Classes for configuration and reading appsettings.json
- **appsettings.json** - Configuration file with connection string

## Prerequisites

1. **Azure Service Bus Namespace** created in Azure
2. **Queue** created in Service Bus (default name: `demo-queue`)
3. **Connection string** from Service Bus

## Configuration

1. Open the `appsettings.json` file
2. Replace `<your-service-bus-name>` with your Service Bus namespace name
3. Replace `<your-shared-access-key-name>` with your access policy name (e.g., RootManageSharedAccessKey)
4. Replace `<your-shared-access-key>` with the access key
5. Adjust the `QueueName` if necessary

Example:
```json
{
  "ConnectionString": "Endpoint=sb://myservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=youraccesskey==",
  "QueueName": "demo-queue"
}
```

## How to Run

### Option 1: Main Menu
```bash
dotnet run
```
Choose option 1 for Sender or 2 for Receiver.

### Option 2: Run Directly with PowerShell Script

**To send messages:**
```bash
.\run.ps1 sender
```

**To receive messages:**
```bash
.\run.ps1 receiver
```

### Option 3: Build and Run Separately

```bash
# Build
dotnet build

# Run Sender
dotnet run --no-build

# In another terminal, run Receiver
dotnet run --no-build
```

## Demo Features

### Sender
- Sends individual messages with custom properties
- Sends batch messages for better performance
- Shows detailed information for each sent message
- Includes structured JSON data in messages

### Receiver
- Processes messages asynchronously
- Shows detailed information for each received message
- Deserializes JSON data from messages
- Handles processing errors
- Allows stopping processing by pressing 'q'

## Message Types

The application sends two types of messages:

1. **Individual Messages**: Sent one at a time with Subject "IndividualMessage"
2. **Batch Messages**: Sent in groups with Subject "BatchMessage"

Each message contains:
- Unique ID
- Sequential number
- Descriptive text
- Timestamp
- Sender information
- Custom properties

## Error Management

The Receiver implements error handling:
- Processing errors abandon the message (returns to queue)
- Connection errors are logged with details
- Option to send error messages to Dead Letter Queue

## Demonstrated Features

- ✅ Individual message sending
- ✅ Batch message sending
- ✅ Asynchronous receiving with ServiceBusProcessor
- ✅ Custom message properties
- ✅ JSON serialization/deserialization
- ✅ Error handling
- ✅ Configuration via appsettings.json
- ✅ Auto-complete disabled for manual control
- ✅ Detailed message information

## Useful Commands

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run
dotnet run

# Clean build
dotnet clean
```

## Notes

- The application uses `AutoCompleteMessages = false` to demonstrate manual control over processing
- Messages are completed only after successful processing
- On error, messages are abandoned and return to the queue
- The Receiver processes one message at a time (`MaxConcurrentCalls = 1`)
