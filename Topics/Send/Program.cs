using RabbitMQ.Client;
using Send;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();


var patternManager = new Patterns(channel);


//patternManager.Fanout();
//patternManager.DirectWithRouting();
//patternManager.Topic();
//patternManager.Headers();