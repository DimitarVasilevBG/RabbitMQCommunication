﻿namespace RabbitMQ
{
    using RabbitMQ.Client;
    using System;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public static class RabbitMQQueues
    {
        public const string ApiAQueue = "api1_queue"; // API A Listen to
        public const string ApiBQueue = "api2_queue"; // API B Listen to
    }

    public interface IRabbitMQService
    {
        Task PublishMessage(string queue, string replyQueue, Message message);
    }

    public class RabbitMQService : IRabbitMQService
    {
        private readonly IConnection _connection;

        public RabbitMQService(IConnectionFactory factory)
        {
            _connection = factory.CreateConnectionAsync().Result;  // Create the connection asynchronously

            // Declare queues
            using var channel = _connection.CreateChannelAsync().Result;
            channel.QueueDeclareAsync(RabbitMQQueues.ApiAQueue, false, false, false, null).Wait();
            channel.QueueDeclareAsync(RabbitMQQueues.ApiBQueue, false, false, false, null).Wait();
        }

        public async Task PublishMessage(string queue, string replyQueue, Message message)
        {            
            using var channel = await _connection.CreateChannelAsync();

            var correlationId = Guid.NewGuid().ToString();

            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            var properties = new BasicProperties
            {
                ReplyTo = replyQueue,
                CorrelationId = correlationId
            };

            await channel.BasicPublishAsync(
                "",               // Default exchange (empty string)
                queue,     // The routing key (queue name to send to)
                false,            // Mandatory flag
                properties,       // Basic properties
                body              // Message body
            );
            Console.WriteLine($"[Producer] Sent message to api1_queue: {jsonMessage}");
        }
    }
}
