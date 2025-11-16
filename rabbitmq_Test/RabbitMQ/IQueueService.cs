namespace rabbitmq_Test.RabbitMQ
{
    public interface IQueueService
    {
        string QueueName { get; }
        Task InitializeAsync();
        Task SendMessageAsync<T>(T message, byte priority) where T : class;
        Task SendBatchAsync<T>(IEnumerable<(T message, byte priority)> messages) where T : class;
        Task PurgeAsync();
        Task DeleteAsync(bool ifUnused = false, bool ifEmpty = false);
        Task CloseAsync();
    }
}
