using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Model.OrderItem;

public class OrderItem
{
    public enum ProductType
    {
        MainFood = 1
    }

    public int Id { get; set; }
    [Required]
    public int OrderId { get; set; }
    [Required]
    public int PetId { get; set; }
    [Required]
    public ProductType Product { get; set; } //1.狗糧
    [Required]
    public int Price { get; set; }
    [Required]
    public int Months { get; set; }
    [ForeignKey("PetId")]
    public Pet.Pet Pet { get; set; }
}