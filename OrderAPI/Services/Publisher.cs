
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace OrderAPI.Publisher;

public class Publisher : IPublisher
{
    public void SendMessage<T>(T message)
    {
        //Create ConnectionFactory
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "user",
            Password = "mypass",
            VirtualHost = "/"
        };

        //Create Connetion
        var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "Order-Queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

        var jsonString = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonString);

        channel.BasicPublish(exchange: "", routingKey: "Order-Queue", null, body: body);

    }
}
