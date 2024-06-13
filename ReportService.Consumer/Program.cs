// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Events;

Console.WriteLine("Hello, World!");

Console.ReadLine();

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest",
    VirtualHost = "/"
};

var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.QueueDeclare(queue: "reports",
                        durable: true,
                        exclusive: true,
                        autoDelete: false,
                        arguments: null);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();

    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine(message);
};

channel.BasicConsume(
    queue: "report",
    autoAck: true,
    consumer: consumer);
