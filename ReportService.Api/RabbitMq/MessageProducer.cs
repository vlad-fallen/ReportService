using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ReportService.Api.RabbitMq
{
    public class MessageProducer : IMessageProducer
    {
        public void SendingMessage<T>(T message)
        {
            //RabbitMQ.Client.Exceptions.BrokerUnreachableException: None of the specified endpoints were reachable
            //не смог запустить с rabbitmq

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };

            var connection = factory.CreateConnection();
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "reports",
                                        durable: true,
                                        exclusive: true,
                                        autoDelete: false,
                                        arguments: null);
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(exchange: "",
                                        routingKey: "reports",
                                        body: body);
            }
        }
    }
}
