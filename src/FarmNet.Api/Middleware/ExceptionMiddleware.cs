using System.Net;
using System.Text.Json;

namespace FarmNet.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi không xử lý được: {Message}", ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var response = new { message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau." };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
