﻿using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "q.events.client1",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

channel.QueueDeclare(queue: "q.events.client2",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");
};
channel.BasicConsume(queue: "q.events.client1", 
                     autoAck: false,
                     consumer: consumer);

//channel.BasicConsume(queue: "q.events.client2",
//                     autoAck: true,
//                     consumer: consumer);




Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();