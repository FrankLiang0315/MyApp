namespace MyApp.Model.Order.Request;

public class OrderUpdateStatusRequest
{
    public int Id { get; set; }
    public Order.OrderStatus Status { get; set; }
}