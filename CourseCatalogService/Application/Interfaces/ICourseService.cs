using CourseCatalogService.Application.Models.DTOs;
using CourseCatalogService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CourseCatalogService.Application.Interfaces
{
    public interface ICourseService
    {
        public Task<CourseResponseDto> AddCourseAsync(CourseRequestDto courseRequestDto);
        public Task<IEnumerable<CourseResponseDto>> GetCoursesAsync();
        public Task<CourseResponseDto?> GetCourseByNameAsync(int id, CancellationToken cancellationToken);
    }
}
