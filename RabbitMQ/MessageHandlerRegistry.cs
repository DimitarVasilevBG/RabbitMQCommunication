using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Interfaces;

namespace RabbitMQ
{
    public abstract class MessageHandlerRegistry
    {
        protected Dictionary<string, Type> _handlers;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        protected MessageHandlerRegistry(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public bool TryGetHandler(string command, out IMessageHandler handler)
        {
            try
            {
                handler = null;

                if (_handlers.TryGetValue(command, out var handlerType))
                {
                    // Resolve the handler in the current scope
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        handler = (IMessageHandler)scope.ServiceProvider.GetRequiredService(handlerType);
                    }

                    return handler != null;
                }

                return false;
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }
    }
}
