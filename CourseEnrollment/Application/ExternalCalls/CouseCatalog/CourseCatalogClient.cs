using CourseEnrollment.Application.Models.DTOs;
using Microsoft.Extensions.Options;
using Shared.Authentication;

namespace CourseEnrollment.Application.ExternalCalls.CouseCatalog
{
    public class CourseCatalogClient(
        HttpClient httpClient,
        IOptions<CourseCatalogSettings> settings,
        IServiceTokenProvider tokenProvider) : ICourseCatalogClient
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly CourseCatalogSettings _settings = settings.Value;
        private readonly IServiceTokenProvider _tokenProvider = tokenProvider;

        public async Task<decimal> GetCoursePriceAsync(int courseId, CancellationToken cancellationToken = default)
        {
            var endpoint = _settings.Endpoints.GetCourseById.Replace("{courseId}", courseId.ToString());

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.GenerateToken());

            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();

            var course = await response.Content.ReadFromJsonAsync<CourseCatalogResponse>(cancellationToken: cancellationToken);
            return course!.Price;
        }
    }
}
