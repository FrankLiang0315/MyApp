using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MyApp.Model.User;

public class User:IdentityUser
{
    [MaxLength(30)]
    public string? FirstName { get; set; }
    [MaxLength(30)]
    public string? LastName { get; set; }
    public ICollection<Pet.Pet> Pets { get; set; }
    
}