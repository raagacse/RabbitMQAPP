using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace InventoryAPI.Subscriber;

public class Consumer : ISubscriber
{
    public void Subscribe()
    {
        Console.WriteLine($"Message Received: Started");
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

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, eventArg) =>
        {
            var body = eventArg.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Message Received: {message}");
        };

        channel.BasicConsume(queue: "Order-Queue", autoAck: true, consumer);
    }
}

