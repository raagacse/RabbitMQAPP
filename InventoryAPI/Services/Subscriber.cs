using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace InventoryAPI.Subscriber;

public class Consumer : ISubscriber
{
    public void Subscribe()
    {
        Console.WriteLine("Message - Started");
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
        channel.ExchangeDeclare(exchange: "Order-Exchange", type: ExchangeType.Direct);
        channel.QueueDeclare(queue: "Order-Queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queue: "Order-Queue", exchange: "Order-Exchange", routingKey: "Order.Init");
        channel.BasicQos(0, 1, false);
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

