using RabbitMQ;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConnectionFactory>(new ConnectionFactory() { HostName = "localhost" });
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();


builder.Services.AddHostedService(provider => 
{
    var factory = provider.GetRequiredService<IConnectionFactory>();
    var service = provider.GetRequiredService<IRabbitMQService>();

    return new RabbitMQConsumer(RabbitMQQueues.ApiAQueue, service, factory); 
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