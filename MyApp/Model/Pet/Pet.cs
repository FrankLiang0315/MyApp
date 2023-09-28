namespace MyApp.Model.Pet;

public class Pet
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string? Name { get; set; }
    public DateTime? Birthday { get; set; }
    public int? Gender { get; set; } //性別 1:公 2:母
    public string? Weight { get; set; }
    public string? Breed { get; set; } //品種
    // public Boolean? IsNeutered { get; set; } //絕育YN
    public int? Posture { get; set; } //體態1:過瘦 2:瘦 3:理想 4:超重 5:過胖
    public int? Activity { get; set; } //活動 1:無激烈運動 2:每日運動少於1h 3:1h~3h 4:3h up
}