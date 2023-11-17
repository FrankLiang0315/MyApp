namespace MyApp.Tools;

public static class Error
{
    public enum Code
    {
        Success = 0000,
        UserCreateFailed = 0001,
        UserAlreadyExists = 0002,
        PasswordNotMatch = 0003,
        UserNotExists = 0004,
        ResetPasswordFailed = 0005
    }

    public static string GetMessage(Code code)
    {
        return code switch
        {
            Code.Success => "",
            Code.UserCreateFailed => "帳號建立失敗",
            Code.UserAlreadyExists => "帳號已存在",
            Code.PasswordNotMatch => "確認密碼不符",
            Code.UserNotExists => "帳號不存在",
            Code.ResetPasswordFailed => "重設密碼錯誤",
            _ => ""
        };
    }
}