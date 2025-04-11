using RabbitMQ.Client;

namespace Send
{
    public class Patterns
    {
        private readonly IModel _channel;

        public Patterns(IModel channel)
        {
            _channel = channel;
        }

        //Publish/Subscribe (Fanout) pattern
        //All messages published by ex.events exchange will be send to all binded queues
        public void Fanout()
        {
            //Create 2 durable queues

            _channel.QueueDeclare(queue: "q.events.client1",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);


            _channel.QueueDeclare(queue: "q.events.client2",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

            //Create exchange

            _channel.ExchangeDeclare("ex.events",
                     type: "fanout",
                     durable: true,
                     autoDelete: false,
                     arguments: null);

            //Bind the queues to the exchange

            _channel.QueueBind(queue: "q.events.client1", exchange: "ex.events", routingKey: string.Empty);
            _channel.QueueBind(queue: "q.events.client2", exchange: "ex.events", routingKey: string.Empty);

        }


        //Publish/Subscribe based on routing
        //All messages published by ex.events.themed exchange will be send to certain queues depending on the message routing key
        public void DirectWithRouting()
        {
            //Create 2 durable queues

            _channel.QueueDeclare(queue: "q.events.client1",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);


            _channel.QueueDeclare(queue: "q.events.client2",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

            //Create exchange

            _channel.ExchangeDeclare("ex.direct.events",
                     type: "direct",
                     durable: true,
                     autoDelete: false,
                     arguments: null);

            //Bind the queues to the exchange
            //First client will recieve only messages with route sport aka sport themed news
            _channel.QueueBind("q.events.client1", "ex.direct.events", "sport");

            //Second client will recieve only messages with route sport or weather aka sport or weather themed news
            _channel.QueueBind("q.events.client2", "ex.direct.events", "sport");
            _channel.QueueBind("q.events.client2", "ex.direct.events", "weather");


        }


        //Publish/Subscribe based on topic
        //All messages published by ex.events.topic exchange will be send to certain queues depending on the message topic
        public void Topic()
        {
            //Create 2 durable queues

            _channel.QueueDeclare(queue: "q.events.client1",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);


            _channel.QueueDeclare(queue: "q.events.client2",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

            //Create exchange

            _channel.ExchangeDeclare("ex.topic.events",
                     type: "topic",
                     durable: true,
                     autoDelete: false,
                     arguments: null);

            //Bind the queues to the exchange
            //This topic will match only one word where the * is, like news.sport.barcelona, there need to be exactly 3 words for the routing to work
            _channel.QueueBind("q.events.client1", "ex.topic.events", "*.sport.*");

            //This topic will match only one word where the * is and whatever is after the #, this way 2 words or more can be matched like event.sport
            _channel.QueueBind("q.events.client2", "ex.topic.events", "*.sport.#");
            //This topic will match only one word where the * is and whatever is after the #
            _channel.QueueBind("q.events.client2", "ex.topic.events", "*.weather.london.*");


        }


        //Publish/Subscribe based on headers
        //All messages published by ex.headers.events exchange will be send to certain queues depending on the header matches
        public void Headers()
        {
            //Create 2 durable queues

            _channel.QueueDeclare(queue: "q.events.client1",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);


            _channel.QueueDeclare(queue: "q.events.client2",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

            //Create exchange

            _channel.ExchangeDeclare("ex.headers.events",
                     type: "headers",
                     durable: true,
                     autoDelete: false,
                     arguments: null);

            //Bind the queues to the exchange
            //This topic will match only one word where the * is, like news.sport.barcelona, there need to be exactly 3 words for the routing to work

            var argumentsClient1 = new Dictionary<string, object>
            {
                { "x-match", "all" },  // All headers must match, if category is sport and source is bbc
                { "category", "sport" },
                { "source", "bbc" }
            };

            var argumentsClient2 = new Dictionary<string, object>
            {
                { "x-match", "any" },  // Any one of the headers must match, if category is sport or source is cnn
                { "category", "sport" },
                { "source", "cnn" }
            };

            _channel.QueueBind("q.events.client1", "ex.headers.events", string.Empty, argumentsClient1);
            _channel.QueueBind("q.events.client2", "ex.headers.events", string.Empty, argumentsClient2);
        }
    }
}
