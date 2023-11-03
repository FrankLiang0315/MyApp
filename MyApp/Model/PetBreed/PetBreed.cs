using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Model.PetBreed;

public class PetBreed
{
    public enum SizeType
    {
        None = 0,
        Small = 1,
        Medium = 2,
        Big = 3,
    }

    public enum BreedType
    {
        None = 0,
        Dog = 1,
        Cat = 2
    }

    public int Id { get; set; }
    
    [Required] 
    public BreedType Type { get; set; } // 1 狗 2 貓
    
    [Required] 
    public SizeType Size { get; set; } // 1 小型犬 2 中型犬 3 大型犬
    
    [Required] 
    [MaxLength(20)] 
    public string Name { get; set; } = "";
}