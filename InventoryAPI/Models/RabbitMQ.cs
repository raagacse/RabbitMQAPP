namespace InventoryAPI.Models;

public class RabbitMQ
{
    public const string RabbitMQConfig = "RabbitMQConfig";
    public string HostName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = string.Empty;
}