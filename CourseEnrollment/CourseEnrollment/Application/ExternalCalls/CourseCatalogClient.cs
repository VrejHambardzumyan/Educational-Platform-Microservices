using CourseEnrollment.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace CourseEnrollment.Application.ExternalCalls
{
    public class CourseCatalogClient : ICourseCatalogClient
    {
        private readonly HttpClient _httpClient;
        private readonly CourseCatalogSettings _settings;
        public CourseCatalogClient(HttpClient httpClient,IOptions<CourseCatalogSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<decimal> GetCoursePriceAsync(int courseId, CancellationToken cancellationToken = default)
        {
            var endpoint = _settings.Endpoints.GetCourseById.Replace("{courseId}", courseId.ToString());

            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();

            var course = await response.Content.ReadFromJsonAsync<CourseCatalogResponse>(cancellationToken: cancellationToken);

            return course!.Price;
        }
    
        public class CourseCatalogResponse
        {
            public int Id { get; set; }
            public decimal Price { get; set; }
        }
    }
}
