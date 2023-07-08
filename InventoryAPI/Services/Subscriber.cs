using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace InventoryAPI.Subscriber;

public class Subscriber : ISubscriber, IDisposable
{
    private IConnection _connection;
    private IModel _channel;
    public Subscriber()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "user",
            Password = "mypass",
            VirtualHost = "/"
        };

        //Create Connetion
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "Order-Exchange", type: ExchangeType.Direct);
        _channel.QueueDeclare(queue: "Order-Queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(queue: "Order-Queue", exchange: "Order-Exchange", routingKey: "Order.Init");
        _channel.BasicQos(0, 1, false);

    }
    public void ReadMessage()
    {
        Console.WriteLine("Message - Started");
        //Create ConnectionFactory

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, eventArg) =>
        {
            var body = eventArg.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Message Received: {message}");
        };

        _channel.BasicConsume(queue: "Order-Queue", autoAck: true, consumer);
    }

    public void Dispose()
    {
        if (_connection.IsOpen)
        {
            _connection.Close();
        }
        if (_channel.IsOpen)
        {
            _channel.Close();
        }
    }


}

