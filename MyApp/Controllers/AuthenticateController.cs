using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyApp.Types;
using MyApp.Model;
using MyApp.Model.Auth;

namespace MyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            var user2 = await _userManager.FindByIdAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new (ClaimTypes.Name, user.UserName),
                    new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

               var token = GetToken(authClaims);
               var returnToken = new JwtSecurityTokenHandler().WriteToken(token);
               var readToken = ReadToken(returnToken);

                return Ok(new
                {
                    token = returnToken,
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            
            
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            if (await _roleManager.RoleExistsAsync(UserRoleTypes.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoleTypes.User);
            }
            return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoleTypes.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoleTypes.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoleTypes.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoleTypes.User));

            if (await _roleManager.RoleExistsAsync(UserRoleTypes.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoleTypes.Admin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoleTypes.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoleTypes.User);
            }
            return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            
            // 生成token
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
        
        private string ReadToken(string? originalToken)
        {
            // 從請求的標頭中獲取 JWT
            string? token = originalToken?.ToString().Replace("Bearer ", "");
            
            // 驗證和解析 JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            
            // 提取用戶資料
            var claims = jwtToken.Claims;
            string? userId = claims.First(claim => claim.Type == ClaimTypes.Name)?.Value;
            string? role = claims.FirstOrDefault(claim => claim.Type == "role")?.Value;
            
            // 在這裡你可以使用用戶資料進行相應的處理
            // 例如，根據角色授權用戶訪問特定資源
            
            return $"User ID: {userId}, Role: {role}";
            // return "123";
        }
    }
}
