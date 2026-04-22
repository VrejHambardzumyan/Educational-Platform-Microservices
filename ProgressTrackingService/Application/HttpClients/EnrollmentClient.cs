using System.Net.Http.Json;

namespace ProgressTrackingService.Application.HttpClients
{
    public class EnrollmentClient(HttpClient http) : IEnrollmentClient
    {
        private readonly HttpClient _http = http;

        /// <summary>
        /// Calls <c>GET /CourseEnrollment/is-enrolled/{courseId}</c> and returns
        /// the boolean result. Treats any non-success response as not enrolled.
        /// </summary>
        public async Task<bool> IsEnrolledAsync(int courseId, CancellationToken ct = default)
        {
            var response = await _http.GetAsync($"CourseEnrollment/is-enrolled/{courseId}", ct);
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<IsEnrolledResponse>(ct);
            return result?.IsEnrolled ?? false;
        }

        private sealed record IsEnrolledResponse(bool IsEnrolled);
    }
}
