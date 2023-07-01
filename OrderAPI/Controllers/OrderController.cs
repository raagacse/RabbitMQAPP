using Microsoft.AspNetCore.Mvc;
using OrderAPI.Models;
using OrderAPI.Publisher;

namespace OrderAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IPublisher _publisher;

    public OrderController(ILogger<OrderController> logger, IPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }

    [HttpPost]
    [Route("NewOrder")]
    public IActionResult NewOrder([FromBody] Order order)
    {
        if (!ModelState.IsValid) return BadRequest();

        _publisher.SendMessage<Order>(order);
        return Ok();
    }

}
