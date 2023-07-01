
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
        await Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("---Exec Async---");
        _subscriber.Subscribe();
        await Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("---Stop Async---");
        await Task.CompletedTask;
    }
}