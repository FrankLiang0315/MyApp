using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using MyApp.Types;
using MyApp.Model;
using MyApp.Model.Auth;
using MyApp.Tools;
using NuGet.Protocol;

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
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                foreach (var userRole in userRoles)
                {
                    Console.WriteLine(userRole);
                }
                var authClaims = new List<Claim>
                {
                    new (ClaimTypes.Name, user.UserName),
                    new (ClaimTypes.Sid, user.Id),
                    new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

               var token = GetToken(authClaims);
               var returnToken = new JwtSecurityTokenHandler().WriteToken(token);
               // var readToken = ReadToken(returnToken);

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
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var userExists = await _userManager.FindByNameAsync(request.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            if (request.Password != request.ConfirmPassword)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Password and Confirm do not match!" });
            }

            IdentityUser user = new()
            {
                Email = request.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = request.Username
            };
            
            
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            if (await _roleManager.RoleExistsAsync(UserRoleTypes.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoleTypes.User);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request)
        {
            var userExists = await _userManager.FindByNameAsync(request.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = request.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = request.Username
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

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
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        
        [HttpPost]
        [Authorize]
        [Route("user-info")]
        public async Task<IActionResult> UserInfo()
        {
            string jwtToken = HttpContext.Request.Headers["Authorization"];
            return Ok(new Response<UserInfo> { Status = "Success", Data = ReadToken(jwtToken) });
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            
            var user = await _userManager.FindByEmailAsync(model.Email);
            
            if (user == null)
            {
                // 使用者不存在，可能顯示錯誤信息
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User not exists!" });
            }
            
            // 生成重設密碼令牌並發送郵件
            var token = HttpUtility.UrlEncode(await _userManager.GeneratePasswordResetTokenAsync(user));
            var callbackUrl = _configuration["URL:Frontend"] + $"/reset-password?email={user.Email}&token={token}";
            
            // HTML 檔案的路徑
            string htmlFilePath = "Files/Email/mail.html";
            
            // 讀取 HTML 檔案的內容並儲存在字串中
            string htmlContent = System.IO.File.ReadAllText(htmlFilePath);
            
            // 現在你可以使用 htmlContent 字串進行操作
            var html = htmlContent.Replace("{resetPasswordUrl}", callbackUrl);
            MailSever sever = new MailSever(_configuration);
            sever.SendMail(model.Email,"Reset Password", html);
            return Ok(new Response { Status = "Success",Message = callbackUrl, Data = html });
        }
        
        [HttpPost]
        [AllowAnonymous]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // 使用者不存在，可能顯示錯誤信息
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User not exists!" });
                }

                // 使用者驗證通過，更新密碼
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    return Ok(new Response { Status = "Success" });
                }

                // 密碼重設失敗，可能顯示錯誤信息
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = $"Reset Error! {result.Errors.FirstOrDefault().ToJson()}" });
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User not exists!" });
        }

        // private function
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
        private UserInfo ReadToken(string? originalToken)
        {
            // 從請求的標頭中獲取 JWT
            string? token = originalToken?.ToString().Replace("Bearer ", "");
            
            // 驗證和解析 JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            
            // 提取用戶資料
            var claims = jwtToken.Claims;
            string? UserId = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
            List<string> Roles = new List<string>();
            
            foreach (var claim in claims.Where((claim)=> claim.Type == ClaimTypes.Role).ToArray())
            {
                Roles.Add(claim.Value);
            }
            
            return new UserInfo {UserId = UserId, Roles = Roles};
        }
    }
}
