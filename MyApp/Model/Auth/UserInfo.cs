using Microsoft.AspNetCore.Identity;

namespace MyApp.Model.Auth;

public class UserInfo
{
    public string UserId { get; set; }
    public List<string> Roles { get; set; }
}