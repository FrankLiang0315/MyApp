using Microsoft.AspNetCore.Mvc;
using MyApp.Model.Auth;

namespace MyApp.Controllers;

public class InfoController: ControllerBase
{
    protected UserInfo? getUserInfo()
    {
        if (HttpContext.Items.TryGetValue("UserInfo", out var userInfo) && userInfo is UserInfo)
        {
            return ((UserInfo)userInfo);
        }

        return null;
    }
}