namespace OrderAPI.Models;

public class Order
{
    public int productId { get; set; }
    public string? productName { get; set; }
    public int count { get; set; }
}