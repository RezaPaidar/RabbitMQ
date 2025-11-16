using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace rabbitmq_Test.RabbitMQ
{
    public class RabbitMQPriorityMessageService(RabbitMqConfig config) : IQueueService, IAsyncDisposable
    {
        private readonly RabbitMqConfig _config = config ?? throw new ArgumentNullException(nameof(config));
        private IConnection? _connection;
        private IChannel? _channel;
        private bool _disposed = false;
        private bool _initialized = false;

        public string QueueName => _config.QueueName;


        public async Task InitializeAsync()
        {
            if (_initialized) return;

            var factory = new ConnectionFactory
            {
                HostName = _config.HostName,
                UserName = _config.UserName,
                Password = _config.Password,
                Port = _config.Port,
                VirtualHost = _config.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: _config.QueueName,
                durable: _config.Durable,
                exclusive: _config.Exclusive,
                autoDelete: _config.AutoDelete,
                arguments: new Dictionary<string, object>
                {
                    { "x-max-priority", _config.MaxPriority }
                });

            _initialized = true;
            Console.WriteLine($"Queue '{_config.QueueName}' initialized successfully.");
        }

        public async Task SendMessageAsync<T>(T message, byte priority) where T : class
        {
            if (!_initialized)
                throw new InvalidOperationException($"Queue '{QueueName}' not initialized. Call InitializeAsync first.");

            if (_channel == null)
                throw new InvalidOperationException("Channel is not available.");

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Priority = priority,
                Persistent = _config.Durable,
                ContentType = "application/json",
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: _config.QueueName,
                mandatory: true,
                basicProperties: properties,
                body: body);

            Console.WriteLine($"Sent to '{QueueName}' with priority {priority}: {JsonSerializer.Serialize(message)}");
        }

        public async Task SendBatchAsync<T>(IEnumerable<(T message, byte priority)> messages) where T : class
        {
            foreach (var (message, priority) in messages)
            {
                await SendMessageAsync(message, priority);
                await Task.Delay(50); //Delete it for the real app
            }
        }

        public async Task PurgeAsync()
        {
            if (!_initialized)
                throw new InvalidOperationException($"Queue '{QueueName}' not initialized. Call InitializeAsync first.");

            if (_channel == null)
                throw new InvalidOperationException("Channel is not available.");

            try
            {
                // Purge the queue (remove all messages)
                var purgeResult = await _channel.QueuePurgeAsync(_config.QueueName);
                Console.WriteLine($"Queue '{QueueName}' purged successfully. Removed {purgeResult} messages.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error purging queue '{QueueName}': {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(bool ifUnused = false, bool ifEmpty = false)
        {
            if (!_initialized)
                throw new InvalidOperationException($"Queue '{QueueName}' not initialized. Call InitializeAsync first.");

            if (_channel == null)
                throw new InvalidOperationException("Channel is not available.");

            try
            {
                // Delete the queue
                var deleteResult = await _channel.QueueDeleteAsync(
                    queue: _config.QueueName,
                    ifUnused: ifUnused,
                    ifEmpty: ifEmpty);

                Console.WriteLine($"Queue '{QueueName}' deleted successfully. Removed {deleteResult} messages during deletion.");

                // Mark as not initialized since queue no longer exists
                _initialized = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting queue '{QueueName}': {ex.Message}");
                throw;
            }
        }

        public async Task CloseAsync()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
                _channel = null;
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                _connection = null;
            }

            _initialized = false;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await CloseAsync();
                _disposed = true;
            }
        }
    }
}
