using System.ComponentModel.DataAnnotations;

namespace MyApp.Model.Order.Request;

public class OrderCreateRequest
{
    [Required]
    public string OrdererName { get; set; }
    [Required]
    public string OrdererTel { get; set; }
    [Required]
    public string OrdererAdd { get; set; }
    [Required]
    public string ReceiverName { get; set; }
    [Required]
    public string ReceiverTel { get; set; }
    [Required]
    public string ReceiverAdd { get; set; }
    [Required]
    public List<Item> Items { get; set; }
}