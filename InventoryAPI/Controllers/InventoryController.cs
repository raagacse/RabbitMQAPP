using InventoryAPI.Subscriber;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class InventoryController : ControllerBase
{
    private readonly ILogger<InventoryController> _logger;
    private readonly ISubscriber _subscriber;

    public InventoryController(ILogger<InventoryController> logger, ISubscriber subscriber)
    {
        _logger = logger;
        _subscriber = subscriber;
    }

}