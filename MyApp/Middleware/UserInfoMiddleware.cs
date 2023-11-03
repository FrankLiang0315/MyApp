using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MyApp.Model.Auth;

namespace MyApp.Middleware;

public class UserInfoMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string originalToken = context.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(originalToken))
        {
            context.Items["UserInfo"] = new UserInfo();
        }
        else
        {
            // 從請求的標頭中獲取 JWT
            string? token = originalToken.Replace("Bearer ", "");

            // 驗證和解析 JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);


            // 提取用戶資料
            var claims = jwtToken.Claims;
            string? UserId = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
            List<string> Roles = new List<string>();

            foreach (var claim in claims.Where((claim) => claim.Type == ClaimTypes.Role).ToArray())
            {
                Roles.Add(claim.Value);
            }

            context.Items["UserInfo"] = new UserInfo { UserId = UserId, Roles = Roles };
        }


        await next(context);
    }
}