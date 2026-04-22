using ProgressTrackingService.Application.Models.DTOs;

namespace ProgressTrackingService.Application.HttpClients
{
    public interface ICatalogClient
    {
        /// <summary>
        /// Returns the list of lessons belonging to <paramref name="courseId"/>.
        /// Returns an empty list when the course does not exist or has no lessons.
        /// </summary>
        Task<IReadOnlyList<CatalogLessonDto>> GetCourseLessonsAsync(int courseId, CancellationToken ct = default);
    }
}
