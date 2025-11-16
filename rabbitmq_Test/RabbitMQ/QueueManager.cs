using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rabbitmq_Test.RabbitMQ
{
    public class QueueManager : IAsyncDisposable
    {
        private readonly Dictionary<string, IQueueService> _queues = new();
        private readonly List<IQueueService> _initializedServices = new();

        public void RegisterQueue(IQueueService queueService)
        {
            _queues[queueService.QueueName] = queueService;
        }

        public async Task InitializeAllQueuesAsync()
        {
            foreach (var queueService in _queues.Values)
            {
                await queueService.InitializeAsync();
                _initializedServices.Add(queueService);
            }
            Console.WriteLine($"Initialized {_queues.Count} queues.");
        }

        public async Task InitializeQueueAsync(string queueName)
        {
            if (_queues.TryGetValue(queueName, out var queueService))
            {
                await queueService.InitializeAsync();
                _initializedServices.Add(queueService);
            }
            else
            {
                throw new ArgumentException($"Queue '{queueName}' is not registered.");
            }
        }

        public IQueueService? GetQueue(string queueName)
        {
            return _queues.GetValueOrDefault(queueName);
        }

        public async Task SendMessageAsync<T>(string queueName, T message, byte priority) where T : class
        {
            var queue = GetQueue(queueName) ?? throw new ArgumentException($"Queue '{queueName}' not found.");
            await queue.SendMessageAsync(message, priority);
        }

        public async Task SendBatchAsync<T>(string queueName, IEnumerable<(T message, byte priority)> messages) where T : class
        {
            var queue = GetQueue(queueName) ?? throw new ArgumentException($"Queue '{queueName}' not found.");
            await queue.SendBatchAsync(messages);
        }

        public IEnumerable<string> GetRegisteredQueues()
        {
            return _queues.Keys;
        }

        public async Task PurgeQueueAsync(string queueName)
        {
            var queue = GetQueue(queueName) ?? throw new ArgumentException($"Queue '{queueName}' not found.");
            await queue.PurgeAsync();
        }

        public async Task DeleteQueueAsync(string queueName, bool ifUnused = false, bool ifEmpty = false)
        {
            var queue = GetQueue(queueName) ?? throw new ArgumentException($"Queue '{queueName}' not found.");
            await queue.DeleteAsync(ifUnused, ifEmpty);

            // Remove from managed queues
            _queues.Remove(queueName);
            _initializedServices.Remove(queue);
        }

        public async Task DeleteAllQueuesAsync(bool ifUnused = false, bool ifEmpty = false)
        {
            Console.WriteLine("Deleting all queues...");

            // Create a copy of the keys to avoid modification during iteration
            var queueNames = _queues.Keys.ToList();

            foreach (var queueName in queueNames)
            {
                try
                {
                    await DeleteQueueAsync(queueName, ifUnused, ifEmpty);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting queue '{queueName}': {ex.Message}");
                }
            }
            Console.WriteLine("All queues deleted.");
        }

        public async Task PurgeAllQueuesAsync()
        {
            Console.WriteLine("Purging all queues...");
            foreach (var queueService in _queues.Values)
            {
                try
                {
                    await queueService.PurgeAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error purging queue '{queueService.QueueName}': {ex.Message}");
                }
            }
            Console.WriteLine("All queues purged.");
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var service in _initializedServices)
            {
                if (service is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else
                {
                    await service.CloseAsync();
                }
            }
            _queues.Clear();
            _initializedServices.Clear();
        }
    }
}
