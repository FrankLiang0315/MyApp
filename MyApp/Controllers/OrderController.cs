using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Model;
using MyApp.Model.Order;
using MyApp.Model.Order.Request;
using MyApp.Model.Order.Response;
using MyApp.Model.OrderItem;

namespace MyApp.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrderController : InfoController
{
    private readonly ApplicationDbContext _context;

    public OrderController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("list")]
    public async Task<ActionResult<Response<List<Order>>>> OrderList()
    {
        var userInfo = getUserInfo();
        var orders = await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Pet)
            .Where((o) => o.UserId == userInfo.UserId).ToListAsync();
        return new Response<List<Order>> { Status = "Success", Data = orders };
    }


    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<Response>> CreateOrder(OrderCreateRequest request)
    {
        var items = await getItemsWithPrice(request.Items);
        var orderItems = new List<OrderItem>();
        var dateTime = DateTime.Now;
        var userInfo = getUserInfo();
        int totalPrice = 0;

        if (userInfo == null)
        {
            return NotFound();
        }

        foreach (var itemWithPrice in items)
        {
            orderItems.Add(new OrderItem
            {
                PetId = itemWithPrice.PetId,
                Product = OrderItem.ProductType.MainFood,
                Price = itemWithPrice.Price,
                Months = itemWithPrice.Months
            });
            totalPrice += itemWithPrice.Price;
        }


        var order = new Order
        {
            UserId = userInfo.UserId,
            OrderNumber = "P" + dateTime.ToString("yyyyMMddHHmmss"),
            Status = Order.OrderStatus.Created,
            OrdererName = request.OrdererName,
            OrdererTel = request.OrdererTel,
            OrdererAdd = request.OrdererAdd,
            ReceiverName = request.ReceiverName,
            ReceiverTel = request.ReceiverTel,
            ReceiverAdd = request.ReceiverAdd,
            Price = totalPrice,
            CreatedAt = dateTime,
            UpdatedAt = dateTime,
            OrderItems = orderItems
        };
        _context.Add(order);

        await _context.SaveChangesAsync();

        return Ok(new Response<Order> { Status = "Success", Data = order });
    }

    [HttpPost]
    [Route("get-items-price")]
    public async Task<ActionResult<Response<OrderGetItemsPriceResponse>>> GetItemsPrice(
        OrderGetItemsPriceRequest request)
    {
        var items = await getItemsWithPrice(request.Items);

        return Ok(new Response<OrderGetItemsPriceResponse>
            { Status = "Success", Data = new OrderGetItemsPriceResponse { Items = items } });
    }

    private async Task<List<ItemWithPrice>> getItemsWithPrice(List<Item> items)
    {
        var itemsWithPrice = new List<ItemWithPrice>();
        foreach (var item in items)
        {
            var pet = await _context.Pets.FindAsync(item.PetId);
            int price = 0;
            double toFloor = pet.DailyGrams / 40;
            var basePrice = (int)Math.Floor(toFloor) * 10 + 500;
            switch (item.Months)
            {
                case 1:
                    price = basePrice;
                    break;
                case 3:
                    price = basePrice * 3 - 300;
                    break;
                case 6:
                    price = basePrice * 6 - 1200;
                    break;
                case 12:
                    price = basePrice * 12 - 2400;
                    break;
                default:
                    price = basePrice;
                    break;
            }

            itemsWithPrice.Add(new ItemWithPrice
            {
                Months = item.Months,
                Product = item.Product,
                PetId = item.PetId,
                Price = price
            });
        }

        return itemsWithPrice;
    }
}