using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Model.Pet;

public class Pet
{
    public enum GenderType
    {
        Male = 1,
        Female = 2,
    }
    
    public enum ActivityType //活動 1:無激烈運動 2:每日運動少於1h 3:1h~3h 4:3h up
    {
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
    }

    

    public int Id { get; set; }
    public string? UserId { get; set; }
    public int PetBreedId { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string Name { get; set; }
    
    [Required]
    public DateTime Birthday { get; set; }
    
    [Required]
    public GenderType Gender { get; set; } //性別 1:公 2:母
    
    [Required]
    public string Weight { get; set; }
    
    [Required]
    public Boolean IsNeutered { get; set; } //絕育YN
    
    public int? Posture { get; set; } //體態1:過瘦 2:瘦 3:理想 4:超重 5:過胖
    
    [Required]
    public int DailyGrams { get; set; }
    
    [Required]
    public ActivityType Activity { get; set; }
    
    [ForeignKey("UserId")]
    public User.User? User { get; set; }
    
    [ForeignKey("PetBreedId")]
    public PetBreed.PetBreed? PetBreed { get; set; }
}