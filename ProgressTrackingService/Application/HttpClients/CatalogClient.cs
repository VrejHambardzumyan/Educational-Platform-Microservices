using System.Net.Http.Json;
using ProgressTrackingService.Application.Models.DTOs;

namespace ProgressTrackingService.Application.HttpClients
{
    public class CatalogClient(HttpClient http) : ICatalogClient
    {
        private readonly HttpClient _http = http;

        /// <summary>
        /// Calls <c>GET /courses/{courseId}/lessons</c> on CourseCatalogService.
        /// Returns an empty list on any non-success response so callers can treat
        /// a missing course the same as a course with no lessons.
        /// </summary>
        public async Task<IReadOnlyList<CatalogLessonDto>> GetCourseLessonsAsync(
            int courseId, CancellationToken ct = default)
        {
            var response = await _http.GetAsync($"courses/{courseId}/lessons", ct);
            if (!response.IsSuccessStatusCode) return [];

            var lessons = await response.Content
                .ReadFromJsonAsync<List<CatalogLessonDto>>(ct);

            return lessons ?? [];
        }
    }
}
