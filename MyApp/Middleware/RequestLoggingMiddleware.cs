using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MyApp.Tools;
using Serilog;

namespace MyApp.Middleware;
public class RequestLoggingMiddleware:IMiddleware
{
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger)
    {
        
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // 复制请求内容以便多次读取
        context.Request.EnableBuffering();

        // 记录请求信息
        var request = await FormatRequest(context.Request);
        
        MyLog.Info("all_request", request);

        // 调用下一个中间件
        await next(context);
    }

    private async Task<string> FormatRequest(HttpRequest request)
    {
        var body = request.Body;
        request.EnableBuffering();
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];

        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        var bodyAsText = Encoding.UTF8.GetString(buffer);
        request.Body.Seek(0, SeekOrigin.Begin);

        return $"{request.Method} {request.Path} {request.QueryString} {bodyAsText}";
    }
}