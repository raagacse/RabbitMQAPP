# RabbitMQApp-DirectExchange-.NetCoreWebAPI
## .Net Core Web API with RabbitMQ 

Create .Net core solution

```
dotnet new sln -n RabbitMQApp
```

Create OrderAPI to producer message
```
dotnet new webapi -n OrderAPI
```

Add RabbitMQ.Client package in producer API

```
dotnet add package RabbitMQ.Client
```

Install RabbitMQ Management console in your system. RabbitMQ provides mangement console UI to see messege queue, exchage and all other things
Below is the docker-compose.yml file that would install RabbitMQ management console and up and run.

```
version: "3.8"
services:
  rabbitmq:
    image: rabbitmq:3.8-management-alpine
    container_name: 'rabbitmq'
    environment:
      - RABBITMQ_DEFAULT_USER=user
      - RABBITMQ_DEFAULT_PASS=mypass
    ports:
      - 5672:5672
      - 15672:15672

```
Run the below command
```
docker-compose up
```
## Publisher API

# RabbitMQ Configuration in Order API

RabbitMQ configured in appsettings.json
```
 "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "user",
    "Password": "mypass",
    "VirtualHost": "/"
  }
```
# IPublisher.cs
```
namespace OrderAPI.Publisher;

public interface IPublisher
{
    void SendMessage<T>(T message);
}

```

Publisher.cs
```
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

```

Program.cs
```
// Add services to the container.
builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection(RabbitMQConfig.RabbitMQ));
builder.Services.AddSingleton<IPublisher, Publisher>();
```

## Subacriber API

# RabbitMQ Configuration in Order API

RabbitMQ configured in appsettings.json
```
 "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "user",
    "Password": "mypass",
    "VirtualHost": "/"
  }
```

# ISubscriber.cs
```
namespace InventoryAPI.Subscriber;

public interface ISubscriber
{
    void ReadMessage();
}
```

Subscriber.cs
```
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

```

# BackgroundService.cs
```
using InventoryAPI.Subscriber;

namespace InventoryAPI.ListenerService;

public class OrderListener : BackgroundService
{
    private readonly ILogger<OrderListener> _logger;
    private readonly ISubscriber _subscriber;

    public OrderListener(ILogger<OrderListener> logger, ISubscriber subscriber)
    {
        _logger = logger;
        _subscriber = subscriber;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("---Start Async---");
        _subscriber.ReadMessage();
        await Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("---Stop Async---");
        await Task.CompletedTask;
    }
}
```

Program.cs
```
// Add services to the container.
builder.Services.AddSingleton<ISubscriber, Subscriber>();
builder.Services.AddHostedService<OrderListener>();
```


