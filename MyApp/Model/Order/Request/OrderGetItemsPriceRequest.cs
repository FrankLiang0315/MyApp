namespace MyApp.Model.Order.Request;


public class Item
{
    public int PetId { get; set; }

    public OrderItem.OrderItem.ProductType Product { get; set; }

    public int Months { get; set; }
}

public class OrderGetItemsPriceRequest
{
    public List<Item> Items { get; set; }
}