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

        public async Task<CourseResponseDto> AddCourseAsync(CourseRequestDto courseRequestDto)
        {
            var course = new Course
            {
                Title = courseRequestDto.Title,
                Description = courseRequestDto.Description,
                DurationInMonth = courseRequestDto.DurationInMonth,
                Price = courseRequestDto.Price,
            };
            var created  = await _courseRepo.AddEntityAsync(course);

            return new CourseResponseDto
            {
                Id = created.Id,
                Title = created.Title,
                DurationInMonth = created.DurationInMonth,
                Price = created.Price,
            };
        }

        public async Task<IEnumerable<CourseResponseDto>> GetCoursesAsync()
        {
            var courses = await _courseRepo.GetAllCoursesAsync();
            return courses.Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Title = c.Title,
                DurationInMonth = c.DurationInMonth,
                Price = c.Price,            
            });

        }

        public async Task<CourseResponseDto?> GetCourseByNameAsync(string courseName, CancellationToken cancellationToken)
        {
            var courses = await _courseRepo.GetCourseByNameAsync(courseName, cancellationToken);
            if (courses == null)
                return null;
            return new CourseResponseDto
            {
                Id = courses.Id,
                Title = courses.Title,
                DurationInMonth = courses.DurationInMonth,
                Price = courses.Price,
            };
        }
    }
}
