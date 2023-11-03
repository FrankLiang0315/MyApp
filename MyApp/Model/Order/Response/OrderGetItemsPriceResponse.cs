namespace MyApp.Model.Order.Response;

public class ItemWithPrice
{
    public int PetId { get; set; }
    
    public OrderItem.OrderItem.ProductType Product { get; set; }
    public int Months { get; set; }
    public int Price { get; set; }
}

public class OrderGetItemsPriceResponse
{
    public List<ItemWithPrice> Items { get; set; }
}