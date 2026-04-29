using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Middleware;

public class ValidateWebhookSignatureFilter(IConfiguration configuration, ILogger<ValidateWebhookSignatureFilter> logger) : IAsyncActionFilter
{
    private readonly string _secret = configuration["WebhookSettings:PaymentCallbackSecret"]
        ?? throw new InvalidOperationException("WebhookSettings:PaymentCallbackSecret is not configured.");

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        context.HttpContext.Request.EnableBuffering();

        var signature = context.HttpContext.Request.Headers["X-Webhook-Signature"].FirstOrDefault();
        if (signature is null)
        {
            logger.LogWarning("Webhook rejected: X-Webhook-Signature header is missing.");
            context.Result = new UnauthorizedObjectResult(new { message = "Missing webhook signature." });
            return;
        }

        using var reader = new StreamReader(context.HttpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.HttpContext.Request.Body.Position = 0;

        var computed = ComputeHmac(body, _secret);
        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computed),
                Encoding.UTF8.GetBytes(signature.ToLowerInvariant())))
        {
            logger.LogWarning("Webhook rejected: signature mismatch. Body length={BodyLength} Received={Received} Computed={Computed}",
                body.Length, signature.ToLowerInvariant(), computed);
            context.Result = new UnauthorizedObjectResult(new { message = "Invalid webhook signature." });
            return;
        }

        await next();
    }

    private static string ComputeHmac(string body, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var bodyBytes = Encoding.UTF8.GetBytes(body);
        using var hmac = new HMACSHA256(keyBytes);
        return Convert.ToHexString(hmac.ComputeHash(bodyBytes)).ToLowerInvariant();
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class ValidateWebhookSignatureAttribute : ServiceFilterAttribute
{
    public ValidateWebhookSignatureAttribute() : base(typeof(ValidateWebhookSignatureFilter)) { }
}
