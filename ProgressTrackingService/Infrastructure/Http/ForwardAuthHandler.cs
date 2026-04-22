using System.Net.Http.Headers;

namespace ProgressTrackingService.Infrastructure.Http
{
    /// <summary>
    /// Delegating handler that copies the incoming Bearer token onto every outbound
    /// service-to-service HTTP request, so downstream services can validate the
    /// original user identity without a separate service account.
    /// </summary>
    public class ForwardAuthHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
    {
        private readonly IHttpContextAccessor _http = httpContextAccessor;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authHeader = _http.HttpContext?.Request.Headers.Authorization.ToString();

            if (!string.IsNullOrWhiteSpace(authHeader) &&
                authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader["Bearer ".Length..];
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
