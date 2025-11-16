using rabbitmq_Test.MyRMQ;
using rabbitmq_Test.RabbitMQ;

namespace rabbitmq_Test;

public record PriorityMessage(string Content, DateTime CreatedAt, string MessageType);

class Program
{
    static async Task Main()
    {
        await using var queueManager = new QueueManager();

        try
        {
            Console.WriteLine("Starting queue cleanup and initialization...");

            // Step 1: Register queues
            queueManager.RegisterQueue(new Qx());
            queueManager.RegisterQueue(new Qy());
            queueManager.RegisterQueue(new Qz());

            // Step 2: Clean up existing queues
            await CleanupAndRebuildQueues(queueManager);

            // Step 3: Re-register and initialize queues after cleanup
            await ReinitializeQueues(queueManager);

            // Step 4: Send test messages
            await SendTestMessages(queueManager);

            Console.WriteLine("\nAll operations completed successfully!");
            Console.WriteLine($"Registered queues: {string.Join(", ", queueManager.GetRegisteredQueues())}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static async Task ReinitializeQueues(QueueManager queueManager)
    {
        Console.WriteLine("\nStep 2: Re-initializing queues...");

        // Clear current registrations and re-register
        var queueNames = queueManager.GetRegisteredQueues().ToList();

        // Re-register all queues (this creates fresh instances)
        queueManager.RegisterQueue(new Qx());
        queueManager.RegisterQueue(new Qy());
        queueManager.RegisterQueue(new Qz());

        // Initialize all queues
        await queueManager.InitializeAllQueuesAsync();
        Console.WriteLine("All queues re-initialized successfully.");
    }

    static async Task CleanupAndRebuildQueues(QueueManager queueManager)
    {
        Console.WriteLine("Step 1: Cleaning up existing queues...");

        var queueNames = queueManager.GetRegisteredQueues().ToList();

        foreach (var queueName in queueNames)
        {
            try
            {
                Console.WriteLine($"\nProcessing queue: {queueName}");

                // Try to initialize the queue first (to check if it exists)
                await queueManager.InitializeQueueAsync(queueName);

                // If we get here, the queue exists, so let's purge it
                Console.WriteLine($"Queue '{queueName}' exists. Purging messages...");
                await queueManager.PurgeQueueAsync(queueName);
                Console.WriteLine($"Queue '{queueName}' purged successfully.");

                //Delete the queue completely and recreate
                Console.WriteLine($"Deleting and recreating queue '{queueName}'...");
                await queueManager.DeleteQueueAsync(queueName);
                Console.WriteLine($"Queue '{queueName}' deleted successfully.");
                
            }
            catch (Exception ex) when (ex.Message.Contains("not found") || ex.Message.Contains("404"))
            {
                // Queue doesn't exist, which is fine - we'll create it
                Console.WriteLine($"Queue '{queueName}' doesn't exist. It will be created during initialization.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not process queue '{queueName}': {ex.Message}");
            }
        }

        Console.WriteLine("\nQueue cleanup completed.");
    }

    static async Task SendTestMessages(QueueManager queueManager)
    {
        Console.WriteLine("\nStep 2: Sending test messages...");

        // Send individual messages to different queues
        var nodeMessage = new { Content = "Qx processing started", Type = "Qx", Timestamp = DateTime.Now };
        await queueManager.SendMessageAsync("Qx", nodeMessage, 10);
        Console.WriteLine("Sent message to Qx");

        var helperMessage = new { Content = "Qy task created", Type = "Qy", Timestamp = DateTime.Now };
        await queueManager.SendMessageAsync("Qy", helperMessage, 50);
        Console.WriteLine("Sent message to Qy");

        var systemMessage = new { Content = "Qz alert", Type = "Qz", Timestamp = DateTime.Now };
        await queueManager.SendMessageAsync("Qz", systemMessage, 200);
        Console.WriteLine("Sent message to Qz");

        // Send batch messages
        var batchMessages = new[]
        {
            (new { Content = "Batch message 1", Type = "Qx", Timestamp = DateTime.Now }, (byte)5),
            (new { Content = "Batch message 2", Type = "Qx", Timestamp = DateTime.Now }, (byte)15),
            (new { Content = "Batch message 3", Type = "Qx", Timestamp = DateTime.Now }, (byte)25)
        };
        await queueManager.SendBatchAsync("Qx", batchMessages);
        Console.WriteLine("Sent batch messages to Qx");

        // Send direct message using queue instance
        var Qx = queueManager.GetQueue("Qx");
        if (Qx != null)
        {
            await Qx.SendMessageAsync(new { Content = "Direct message", Type = "Direct", Timestamp = DateTime.Now }, 150);
            Console.WriteLine("Sent direct message to Qx");
        }

        Console.WriteLine("All test messages sent successfully!");
    }
}