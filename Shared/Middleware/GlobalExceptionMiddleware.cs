using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Shared.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteJsonAsync(context, 404, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteJsonAsync(context, 409, ex.Message);
        }
        catch (ArgumentException ex)
        {
            await WriteJsonAsync(context, 400, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteJsonAsync(context, 500, "An unexpected error occurred.");
        }
    }

    private static Task WriteJsonAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(new { message });
    }
}
