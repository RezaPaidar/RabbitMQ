using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rabbitmq_Test
{
    public static class MessageGenerator
    {
        public static IEnumerable<(PriorityMessage message, byte priority)> GenerateInitialBatch()
        {
            return new[]
            {
                (new PriorityMessage("System alert: CPU high", DateTime.Now, "Alert"), (byte)15),
                (new PriorityMessage("User order completed", DateTime.Now, "Order"), (byte)10),
                (new PriorityMessage("Background task result", DateTime.Now, "Background"), (byte)1),
                (new PriorityMessage("Payment processed", DateTime.Now, "Payment"), (byte)20),
                (new PriorityMessage("Log entry", DateTime.Now, "Log"), (byte)18)
            };
        }

        public static IEnumerable<(PriorityMessage message, byte priority)> GenerateEmergencyMessages()
        {
            return new[]
            {
                (new PriorityMessage("EMERGENCY: System failure detected!", DateTime.Now, "Emergency"), (byte)25),
                (new PriorityMessage("URGENT: Database connection lost!", DateTime.Now, "Emergency"), (byte)30),
                (new PriorityMessage("CRITICAL: Security breach detected!", DateTime.Now, "Security"), (byte)255)
            };
        }

        public static IEnumerable<(PriorityMessage message, byte priority)> GenerateNormalMessages(int count = 5)
        {
            var random = new Random();
            var messageTypes = new[] { "Log", "Info", "Debug", "Trace", "Metric" };

            for (int i = 0; i < count; i++)
            {
                var priority = (byte)random.Next(1, 10);
                var messageType = messageTypes[random.Next(messageTypes.Length)];
                yield return (new PriorityMessage($"{messageType} message #{i + 1}", DateTime.Now, messageType), priority);
            }
        }

        public static (PriorityMessage message, byte priority) CreateCustomMessage(string content, string messageType, byte priority)
        {
            return (new PriorityMessage(content, DateTime.Now, messageType), priority);
        }
    }
}
