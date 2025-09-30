using CourseCatalogService.Application.Models.DTOs;
using CourseCatalogService.Infrastructure.Entities;

namespace CourseCatalogService.Application.Interfaces
{
    public interface ICourseService
    {
        public Task<CourseResponseDTO> AddCourseAsync(CourseRequestDTO courseRequestDto);
        public Task<IEnumerable<CourseResponseDTO>> GetCoursesAsync();
        public Task<CourseResponseDTO?> GetCourseByIdAsync(int id, CancellationToken cancellationToken);
    }
}
