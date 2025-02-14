using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace RabbitMQ
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly string _queue;
        private IConnection _connection;
        private IChannel _channel;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IConnectionFactory _factory;


        public RabbitMQConsumer(string queue, IRabbitMQService rabbitMQService, IConnectionFactory factory)
        {
            _rabbitMQService = rabbitMQService;
            _factory = factory;
            _queue = queue;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            // Declare the queue
            await _channel.QueueDeclareAsync(queue: _queue, durable: false, exclusive: false, autoDelete: false, arguments: null);

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string projectName = Assembly.GetEntryAssembly()?.GetName().Name;

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"{projectName} Received: {message}");

                var replyProps = ea.BasicProperties;

                // Send a response back to API1
                var responseMessage = $"Response from {projectName}: {message}";

                if(string.IsNullOrEmpty(replyProps.ReplyTo) == false)
                    await _rabbitMQService.PublishMessage(replyProps.ReplyTo, string.Empty, responseMessage);

                await Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(queue: _queue, autoAck: true, consumer: consumer);


            Console.WriteLine($"{projectName} Waiting for messages...");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();

            await base.StopAsync(cancellationToken);
        }
    }
}
