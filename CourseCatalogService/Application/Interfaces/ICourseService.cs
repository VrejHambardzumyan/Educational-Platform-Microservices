using CourseCatalogService.Application.Models.DTOs;
using Shared.Models;

namespace CourseCatalogService.Application.Interfaces
{
    public interface ICourseService
    {
        Task<CourseResponseDto> AddCourseAsync(CourseRequestDto courseRequestDto, int instructorId);
        Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(int page, int pageSize, string? category = null, string? search = null);
        Task<CourseResponseDto?> GetCourseByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<CourseResponseDto>> GetByInstructorIdAsync(int instructorId, CancellationToken cancellationToken);
        Task<CourseResponseDto?> UpdateCourseAsync(int id, CourseUpdateRequestDto dto, int instructorId);
        Task<bool> DeleteCourseAsync(int id, int userId, bool isAdmin);
        Task<bool> PublishCourseAsync(int id, int instructorId);
    }
}
