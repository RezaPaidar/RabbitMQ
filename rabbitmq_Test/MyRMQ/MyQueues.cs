using rabbitmq_Test.RabbitMQ;

namespace rabbitmq_Test.MyRMQ
{
    public class Qx : IQueueService
    {
        private readonly RabbitMQPriorityMessageService _messageService;
        public string QueueName => "Qx";

        public Qx()
        {
            var config = new RabbitMqConfig()
            {
                QueueName = "Qx",
                Durable = true,
                MaxPriority = 250,
            };
            _messageService = new RabbitMQPriorityMessageService(config);
        }

        public Task InitializeAsync() => _messageService.InitializeAsync();

        public Task SendMessageAsync<T>(T message, byte priority) where T : class
            => _messageService.SendMessageAsync(message, priority);

        public Task SendBatchAsync<T>(IEnumerable<(T message, byte priority)> messages) where T : class
            => _messageService.SendBatchAsync(messages);

        public Task PurgeAsync() => _messageService.PurgeAsync();
        public Task DeleteAsync(bool ifUnused = false, bool ifEmpty = false)
            => _messageService.DeleteAsync(ifUnused, ifEmpty);

        public Task CloseAsync() => _messageService.CloseAsync();

    }
    public class Qy : IQueueService
    {
        private readonly RabbitMQPriorityMessageService _messageService;

        public string QueueName => "Qy";

        public Qy()
        {
            var config = new RabbitMqConfig()
            {
                QueueName = "Qy",
                Durable = true,
                MaxPriority = 250,
            };
            _messageService = new RabbitMQPriorityMessageService(config);
        }

        public Task InitializeAsync() => _messageService.InitializeAsync();
        public Task SendMessageAsync<T>(T message, byte priority) where T : class
            => _messageService.SendMessageAsync(message, priority);
        public Task SendBatchAsync<T>(IEnumerable<(T message, byte priority)> messages) where T : class
            => _messageService.SendBatchAsync(messages);

        public Task PurgeAsync() => _messageService.PurgeAsync();
        public Task DeleteAsync(bool ifUnused = false, bool ifEmpty = false)
            => _messageService.DeleteAsync(ifUnused, ifEmpty);
        public Task CloseAsync() => _messageService.CloseAsync();
    }

    public class Qz : IQueueService
    {
        private readonly RabbitMQPriorityMessageService _messageService;

        public string QueueName => "Qz";

        public Qz()
        {
            var config = new RabbitMqConfig()
            {
                QueueName = "Qz",
                Durable = true,
                MaxPriority = 250,
            };
            _messageService = new RabbitMQPriorityMessageService(config);
        }

        public Task InitializeAsync() => _messageService.InitializeAsync();
        public Task SendMessageAsync<T>(T message, byte priority) where T : class
            => _messageService.SendMessageAsync(message, priority);
        public Task SendBatchAsync<T>(IEnumerable<(T message, byte priority)> messages) where T : class
            => _messageService.SendBatchAsync(messages);

        public Task PurgeAsync() => _messageService.PurgeAsync();
        public Task DeleteAsync(bool ifUnused = false, bool ifEmpty = false)
            => _messageService.DeleteAsync(ifUnused, ifEmpty);
        public Task CloseAsync() => _messageService.CloseAsync();
    }
}
