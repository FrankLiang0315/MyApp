using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Model;
using MyApp.Model.User;

namespace MyApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : InfoController
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    // private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();

        users = users.Where(u => _userManager.GetRolesAsync(u).Result.Contains("Admin")).ToList();

        return Ok(new Response<List<User>> { Status = "Success", Data = users });
    }
}