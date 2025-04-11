using APIServiceB.Handlers;
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Interfaces;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConnectionFactory>(new ConnectionFactory() { HostName = "localhost" });
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();


// Register all handlers dynamically
var handlerTypes = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => typeof(IMessageHandler).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

foreach (var handler in handlerTypes)
{
    builder.Services.AddScoped(handler);  // Register each handler
}

builder.Services.AddSingleton<HandlersRegistry>();

// Register background listener
builder.Services.AddHostedService(provider =>
{
    var factory = provider.GetRequiredService<IConnectionFactory>();
    var service = provider.GetRequiredService<IRabbitMQService>();
    var handlersRegistry = provider.GetRequiredService<HandlersRegistry>();

    return new RabbitMQConsumer(RabbitMQQueues.ApiBQueue, service, factory, handlersRegistry);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
