using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Text.Json;

namespace RabbitMQ
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly string _queue;
        private IConnection _connection;
        private IChannel _channel;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IConnectionFactory _factory;
        private readonly MessageHandlerRegistry _messageHandlerRegistry;

        public RabbitMQConsumer(string queue, IRabbitMQService rabbitMQService, IConnectionFactory factory, MessageHandlerRegistry messageHandlerRegistry)
        {
            _rabbitMQService = rabbitMQService;
            _factory = factory;
            _messageHandlerRegistry = messageHandlerRegistry;
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
                try
                {
                    var body = ea.Body.ToArray();
                    var message = JsonSerializer.Deserialize<Message>(body);
                    var jsonMessage = JsonSerializer.Serialize(message);

                    if (message == null)
                        throw new Exception("Invalid Message!");


                    Console.WriteLine($"{projectName} Received: {jsonMessage}");


                    if(string.IsNullOrEmpty(message.Command) == false) // Process the request
                    {

                        if (_messageHandlerRegistry.TryGetHandler(message.Command, out var handler))
                        {
                            await handler.HandleAsync(message.Data);

                            var responseMessage = new Message { Data = $"Response from {projectName}: {handler.GetType().Name} was executed", IsSuccessful = true };

                            var replyProps = ea.BasicProperties; 
                            if (string.IsNullOrEmpty(replyProps.ReplyTo) == false)
                                await _rabbitMQService.PublishMessage(replyProps.ReplyTo, string.Empty, responseMessage);

                        }
                        else
                        {
                            Console.WriteLine($"[Consumer] Unknown command: {message.Command}");
                        }
                    }


                    await Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    // LOG ERROR

                    var responseMessage = new Message { Data =  $"An error occured while processing the message, Error message: {ex.Message}", IsSuccessful = false };
                    await _rabbitMQService.PublishMessage(ea.BasicProperties.ReplyTo, string.Empty, responseMessage);

                }
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
