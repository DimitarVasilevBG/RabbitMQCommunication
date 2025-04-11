using RabbitMQ.Interfaces;

namespace APIServiceB.Handlers
{
    public class ProcessOrderHandler : IMessageHandler
    {
        public async Task HandleAsync(string data)
        {
            throw new Exception("Could not process the current order!");
        }
    }
}
