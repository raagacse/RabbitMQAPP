namespace OrderAPI.Models;

public class RabbitMQConfig
{
    public const string RabbitMQ = "RabbitMQ";
    public string HostName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = string.Empty;
}
