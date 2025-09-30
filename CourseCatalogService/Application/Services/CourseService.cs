using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Application.Models.DTOs;
using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CourseCatalogService.Application.Services
{
    public class CourseService(ICourseRepository courseRepo) : ICourseService
    {
        private readonly ICourseRepository _courseRepo = courseRepo;

        public async Task<CourseResponseDTO> AddCourseAsync(CourseRequestDTO courseRequestDto)
        {
            var course = new Course
            {
                Title = courseRequestDto.Title,
                Description = courseRequestDto.Description,
                DurationInMonth = courseRequestDto.DurationInMonth,
                Price = courseRequestDto.Price,
            };
            var created  = await _courseRepo.AddEntityAsync(course);

            return new CourseResponseDTO
            {
                Id = created.Id,
                Title = created.Title,
                DurationInMonth = created.DurationInMonth,
                Price = created.Price,
            };
        }

        public async Task<IEnumerable<CourseResponseDTO>> GetCoursesAsync()
        {
            var courses = await _courseRepo.GetAllCoursesAsync();
            return courses.Select(c => new CourseResponseDTO
            {
                Id = c.Id,
                Title = c.Title,
                DurationInMonth = c.DurationInMonth,
                Price = c.Price,            
            });

        }

        public async Task<CourseResponseDTO?> GetCourseByIdAsync(int id, CancellationToken cancellationToken)
        {
            var courses = await _courseRepo.GetCourseByIdAsync(id, cancellationToken);
            return new CourseResponseDTO
            {
                Id = courses.Id,
                Title = courses.Title,
                DurationInMonth = courses.DurationInMonth,
                Price = courses.Price,
            };
        }
    }
}
