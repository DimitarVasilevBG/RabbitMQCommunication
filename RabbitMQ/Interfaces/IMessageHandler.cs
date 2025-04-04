namespace RabbitMQ.Interfaces
{
    public interface IMessageHandler
    {
        Task HandleAsync(string data);
    }
}
