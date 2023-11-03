using Microsoft.AspNetCore.Routing.Constraints;
using Serilog;

namespace MyApp.Tools;

public static class MyLog
{
    public static void Info(string fileName, string text)
    {
        string filePath = $"Logs/{fileName}.txt";
        var log = new LoggerConfiguration()
            .WriteTo.File(filePath) // 指定日志文件的路径
            .CreateLogger();

        log.Information(text);
    }
}