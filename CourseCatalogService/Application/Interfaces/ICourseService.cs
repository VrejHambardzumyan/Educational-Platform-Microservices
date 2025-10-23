using CourseCatalogService.Application.Models.DTOs;

namespace CourseCatalogService.Application.Interfaces
{
    public interface ICourseService
    {
        Task<CourseResponseDto> AddCourseAsync(CourseRequestDto courseRequestDto);
        Task<IEnumerable<CourseResponseDto>>? GetCoursesAsync();
        Task<CourseResponseDto?> GetCourseByIdAsync(int id, CancellationToken cancellationToken);
    }
}
