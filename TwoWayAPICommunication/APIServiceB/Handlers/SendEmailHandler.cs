using RabbitMQ.Interfaces;

namespace APIServiceB.Handlers
{
    public class SendEmailHandler : IMessageHandler
    {
        public async Task HandleAsync(string data)
        {
            Console.WriteLine($"Sending email to: {data}");
            await Task.Delay(500); // Simulate email sending
        }
    }
}
