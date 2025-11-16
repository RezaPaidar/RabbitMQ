# RabbitMQ Priority Queue Manager

A robust .NET application demonstrating RabbitMQ priority queue functionality with dynamic queue management, batch operations, and clean architecture.

---

## Features

- **Priority Queue Support:** Messages with higher priority are processed first  
- **Dynamic Queue Management:** Create, purge, and delete queues programmatically  
- **Batch Operations:** Send multiple messages with different priorities  
- **Clean Architecture:** Separation of concerns with interfaces and services  
- **Flexible Configuration:** Configure multiple queues with different settings  
- **Durable Queues:** Messages persist across broker restarts  
- **Type Safety:** Generic methods for safe message handling  

---

## Queue Structure

The application manages three main queues:

- **Qx:** Queue processing tasks  
- **Qy:** Queue tasks and services  
- **Qz:** Queue alerts and critical operations  

All queues support message priorities **0–255**, with higher numbers meaning higher priority.

---

## Installation

### **Prerequisites**
- .NET **8.0 SDK**  
- RabbitMQ **3.8+** (with priority queue support)  
- `RabbitMQ.Client` **7.1.2+**

---

### **Setup**

#### Install RabbitMQ.Client
```bash
dotnet add package RabbitMQ.Client --version 7.1.2
```

Configure **RabbitMQ Connection**
```
var config = new RabbitMqConfig
{
    HostName = "localhost",
    UserName = "guest", 
    Password = "guest",
    Port = 5672,
    VirtualHost = "/",
    QueueName = "Qx", // or Qy, Qz
    MaxPriority = 255,
    Durable = true
};
```
---

### Basic Setup

```
// Create queue manager
await using var queueManager = new QueueManager();

// Register queues
queueManager.RegisterQueue(new Qx());
queueManager.RegisterQueue(new Qy());
queueManager.RegisterQueue(new Qz());

// Initialize all queues
await queueManager.InitializeAllQueuesAsync();
```
---

## Sending Messages
### Single Message
```
var message = new { Content = "Task started", Type = "Qx", Timestamp = DateTime.Now };
await queueManager.SendMessageAsync("Qx", message, priority: 100);
```

## Batch Messages
```
var batchMessages = new[]
{
    (new { Content = "Low priority", Type = "Qx" }, (byte)10),
    (new { Content = "High priority", Type = "Qx" }, (byte)200),
    (new { Content = "Medium priority", Type = "Qx" }, (byte)50)
};

await queueManager.SendBatchAsync("Qx", batchMessages);
```
---

## Queue Management
### Purge Queue

```
await queueManager.PurgeQueueAsync("Qx");
```

### Delete Queue
```
await queueManager.DeleteQueueAsync("Qx");
```

### Cleanup and Rebuild
```
// Clean all queues and recreate
await CleanupAndRebuildQueues(queueManager);
```

## Architecture
### Core Components

- **IQueueService** – Defines queue operations
- **RabbitMQPriorityMessageService** – Main service implementation
- **QueueManager** – Manages multiple queues
- **RabbitMqConfig** – Configuration model

### Queue Services
- **Qx** – Node processing
- **Qy** – Helper tasks
- **Qz** – System alerts

---

## Message Priority System

| Priority Range | Use Case         | Example                        |
|----------------|------------------|--------------------------------|
| **200–255**    | Critical/Urgent  | System failures, Emergency     |
| **100–199**    | High             | Time-sensitive operations      |
| **50–99**      | Medium           | Normal tasks                   |
| **1–49**       | Low              | Background tasks               |
| **0**          | Default          | Fallback priority              |

---

## Error Handling
Handles:
- Connection failures
- Queue initialization errors
- Message publishing issues
- Network timeouts

---

## Monitoring
Use RabbitMQ Management UI:
- Open http://localhost:15672
- View queues: Qx, Qy, Qz
- Monitor message counts and consumer status

---

## Best Practices
- Use appropriate priority ranges
- Use try/catch around operations
- Clean resources with IAsyncDisposable
- Monitor queue depth regularly
- Use meaningful message types

---

## Logs
```
try
{
    await queueManager.SendMessageAsync("Qx", message, priority);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Contributing
- Fork the repo
- Create a feature branch
- Make your changes
- Add tests
- Submit a PR

## License
This project is licensed under the MIT License.

--
If you'd like, I can also:

Add badges (NuGet, .NET, RabbitMQ, License)  
Add a Table of Contents  
Add images or diagrams  
Add GitHub Actions CI sample  

Just tell me :)
