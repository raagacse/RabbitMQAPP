
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OrderAPI.Models;
using RabbitMQ.Client;

namespace OrderAPI.Publisher;

public class Publisher : IPublisher
{
    private readonly IOptionsMonitor<RabbitMQConfig> _options;
    private IConnection _connection;
    private IModel _channel;

    public Publisher(IOptionsMonitor<RabbitMQConfig> options)
    {
        _options = options;

        //Create ConnectionFactory
        var factory = new ConnectionFactory()
        {
            HostName = _options.CurrentValue.HostName,
            UserName = _options.CurrentValue.UserName,
            Password = _options.CurrentValue.Password,
            VirtualHost = _options.CurrentValue.VirtualHost
        };

        //Create Connetion
        _connection = factory.CreateConnection();

        //Create Channel
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "Order-Exchange", type: ExchangeType.Direct);

    }

    public void SendMessage<T>(T message)
    {
        var jsonString = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonString);

        _channel.BasicPublish(exchange: "Order-Exchange", routingKey: "Order.Init", null, body: body);

    }
}
