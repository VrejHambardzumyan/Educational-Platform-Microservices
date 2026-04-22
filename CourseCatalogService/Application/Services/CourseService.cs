using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Application.Models.DTOs;
using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;

namespace CourseCatalogService.Application.Services
{
    public class CourseService(ICourseRepository courseRepo) : ICourseService
    {
        private readonly ICourseRepository _courseRepo = courseRepo;

        public async Task<CourseResponseDto> AddCourseAsync(CourseRequestDto courseRequestDto, int instructorId)
        {
            var course = new Course
            {
                Title = courseRequestDto.Title,
                Description = courseRequestDto.Description,
                DurationInMonth = courseRequestDto.DurationInMonth,
                Price = courseRequestDto.Price,
                InstructorId = instructorId,
                Category = courseRequestDto.Category,
                Status = CourseStatus.Draft
            };
            var created = await _courseRepo.AddEntityAsync(course);
            return MapToDto(created);
        }

        public async Task<PagedResponseDto<CourseResponseDto>> GetCoursesAsync(
            int page, int pageSize, string? category = null, string? search = null)
        {
            var (courses, totalCount) = await _courseRepo.GetCoursesAsync(page, pageSize, category, search);
            return new PagedResponseDto<CourseResponseDto>
            {
                Items = courses.Select(MapToDto),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<CourseResponseDto?> GetCourseByIdAsync(int id, CancellationToken cancellationToken)
        {
            var course = await _courseRepo.GetCourseByIdAsync(id, cancellationToken);
            if (course == null) return null;
            return MapToDto(course);
        }

        public async Task<IEnumerable<CourseResponseDto>> GetByInstructorIdAsync(int instructorId, CancellationToken cancellationToken)
        {
            var courses = await _courseRepo.GetByInstructorIdAsync(instructorId, cancellationToken);
            return courses.Select(MapToDto);
        }

        public async Task<CourseResponseDto?> UpdateCourseAsync(int id, CourseUpdateRequestDto dto, int instructorId)
        {
            var course = await _courseRepo.GetCourseByIdAsync(id, default);
            if (course == null || course.InstructorId != instructorId) return null;

            if (!string.IsNullOrWhiteSpace(dto.Title)) course.Title = dto.Title;
            if (!string.IsNullOrWhiteSpace(dto.Description)) course.Description = dto.Description;
            if (dto.DurationInMonth.HasValue) course.DurationInMonth = dto.DurationInMonth.Value;
            if (dto.Price.HasValue) course.Price = dto.Price.Value;
            if (dto.Category != null) course.Category = dto.Category;
            if (dto.ThumbnailUrl != null) course.ThumbnailUrl = dto.ThumbnailUrl;

            await _courseRepo.UpdateAsync(course);
            return MapToDto(course);
        }

        public async Task<bool> DeleteCourseAsync(int id, int userId, bool isAdmin)
        {
            var course = await _courseRepo.GetCourseByIdAsync(id, default);
            if (course == null) return false;
            if (!isAdmin && course.InstructorId != userId) return false;

            course.IsDeleted = true;
            await _courseRepo.UpdateAsync(course);
            return true;
        }

        public async Task<bool> PublishCourseAsync(int id, int instructorId)
        {
            var course = await _courseRepo.GetCourseByIdAsync(id, default);
            if (course == null || course.InstructorId != instructorId) return false;

            course.Status = CourseStatus.Published;
            await _courseRepo.UpdateAsync(course);
            return true;
        }

        private static CourseResponseDto MapToDto(Course c) => new()
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            DurationInMonth = c.DurationInMonth,
            Price = c.Price,
            InstructorId = c.InstructorId,
            Status = c.Status.ToString(),
            Category = c.Category,
            ThumbnailUrl = c.ThumbnailUrl,
            AverageRating = c.AverageRating,
            RatingCount = c.RatingCount,
            CreatedAt = c.CreatedAt
        };
    }
}
