using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Model.Order;

public class Order
{
    public enum OrderStatus
    {
        Created = 0,
        PendingPayment = 1,
        InProgress = 2,
        PendingLogistics = 3,
        Shipping = 4,
        Completed = 5
    }
    
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string OrderNumber { get; set; }
    
    [Required]
    public OrderStatus Status { get; set; } // 0.創建 1.待付款 2.處理中 3.運送中 4.完成
    
    [Required]
    [MaxLength(20)]
    public string OrdererName { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string OrdererTel { get; set; }
    
    [Required]
    [MaxLength(300)]
    public string OrdererAdd { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string ReceiverName { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string ReceiverTel { get; set; }
    
    [Required]
    [MaxLength(300)]
    public string ReceiverAdd { get; set; }
    
    [Required]
    public int Price { get; set; }

    public int PaymentCount { get; set; } = 0;

    [MaxLength(30)]
    public string? PaymentNumber { get; set; }

    [MaxLength(30)]
    public string? PaymentManufacturer { get; set; }

    [MaxLength(30)]
    public string? PaymentType { get; set; }
    
    [MaxLength(30)]
    public string? LogisticsNumber { get; set; }
    
    [MaxLength(30)]
    public string? LogisticsManufacturer { get; set; }
    
    [MaxLength(30)]
    public string? LogisticsType { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public DateTime UpdatedAt { get; set; }
    
    [ForeignKey("UserId")]
    public User.User? User { get; set; }
    
    public ICollection<OrderItem.OrderItem>? OrderItems { get; set; }
}