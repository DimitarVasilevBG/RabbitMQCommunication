namespace RabbitMQ
{
    public class Message
    {
        public string Command { get; set; }
        public string Data { get; set; }
        public bool? IsSuccessful { get; set; }
    }
}
