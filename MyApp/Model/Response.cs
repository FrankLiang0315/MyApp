using MyApp.Tools;

namespace MyApp.Model;

public class Response : Response<string>
{
}



public class Response<T>
{
    public T GetValue()
    {
        // 使用 default(T) 取得 T 的預設值
        return default(T);
    }

    public Response()
    {
        Console.WriteLine(ErrorCode);
        if (ErrorCode != Error.Code.Success)
        {
            Message = Error.GetMessage(ErrorCode);
        }

    }

    public string? Status { get; set; }
    public string? Message { get; set; }

    public Error.Code ErrorCode { get; set; } = Error.Code.Success;

    public T? Data { get; set; }
}