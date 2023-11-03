namespace MyApp.Model.Pet.View;

public class PetVM
{
    public int Id { get; set; }
    public int PetBreadId { get; set; }
    public string? Breed { get; set; }
    public string? Name { get; set; }
    public DateTime? Birthday { get; set; }
    public Pet.GenderType Gender { get; set; } //性別 1:公 2:母
    public string? Weight { get; set; }
    public Boolean? IsNeutered { get; set; } //絕育YN
    public Pet.ActivityType Activity { get; set; } //活動 1:無激烈運動 2:每日運動少於1h 3:1h~3h 4:3h up
}