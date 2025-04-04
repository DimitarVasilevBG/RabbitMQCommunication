using RabbitMQ;
using RabbitMQ.Interfaces;
using System.Reflection;

namespace APIServiceA.Handlers
{
    public class HandlersRegistry : MessageHandlerRegistry
    {
        public HandlersRegistry(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
            _handlers = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IMessageHandler).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToDictionary(
                    type => type.Name.Replace("Handler", ""), // Use class name as command key
                    type => type
                );
        }
    }
}
