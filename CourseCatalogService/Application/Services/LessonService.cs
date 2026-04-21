using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Application.Models.DTOs;
using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;

namespace CourseCatalogService.Application.Services
{
    public class LessonService(ILessonRepository lessonRepo, ICourseRepository courseRepo) : ILessonService
    {
        private readonly ILessonRepository _lessonRepo = lessonRepo;
        private readonly ICourseRepository _courseRepo = courseRepo;

        public async Task<LessonResponseDto> AddLessonAsync(int courseId, LessonRequestDto dto, int instructorId)
        {
            var course = await _courseRepo.GetCourseByIdAsync(courseId, default);
            if (course == null || course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("Course not found or you don't own it.");

            var lesson = new Lesson
            {
                CourseId = courseId,
                Title = dto.Title,
                Description = dto.Description,
                VideoUrl = dto.VideoUrl,
                OrderIndex = dto.OrderIndex,
                DurationInMinutes = dto.DurationInMinutes
            };

            var created = await _lessonRepo.AddAsync(lesson);
            return MapToDto(created);
        }

        public async Task<IEnumerable<LessonResponseDto>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default)
        {
            var lessons = await _lessonRepo.GetByCourseIdAsync(courseId, cancellationToken);
            return lessons.Select(MapToDto);
        }

        public async Task<LessonResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var lesson = await _lessonRepo.GetByIdAsync(id, cancellationToken);
            return lesson == null ? null : MapToDto(lesson);
        }

        public async Task<LessonResponseDto?> UpdateLessonAsync(int id, LessonRequestDto dto, int instructorId)
        {
            var lesson = await _lessonRepo.GetByIdAsync(id);
            if (lesson == null) return null;

            var course = await _courseRepo.GetCourseByIdAsync(lesson.CourseId, default);
            if (course == null || course.InstructorId != instructorId) return null;

            lesson.Title = dto.Title;
            if (dto.Description != null) lesson.Description = dto.Description;
            if (dto.VideoUrl != null) lesson.VideoUrl = dto.VideoUrl;
            lesson.OrderIndex = dto.OrderIndex;
            lesson.DurationInMinutes = dto.DurationInMinutes;

            await _lessonRepo.UpdateAsync(lesson);
            return MapToDto(lesson);
        }

        public async Task<bool> DeleteLessonAsync(int id, int instructorId, bool isAdmin)
        {
            var lesson = await _lessonRepo.GetByIdAsync(id);
            if (lesson == null) return false;

            if (!isAdmin)
            {
                var course = await _courseRepo.GetCourseByIdAsync(lesson.CourseId, default);
                if (course == null || course.InstructorId != instructorId) return false;
            }

            return await _lessonRepo.DeleteAsync(id);
        }

        private static LessonResponseDto MapToDto(Lesson l) => new()
        {
            Id = l.Id,
            CourseId = l.CourseId,
            Title = l.Title,
            Description = l.Description,
            VideoUrl = l.VideoUrl,
            OrderIndex = l.OrderIndex,
            DurationInMinutes = l.DurationInMinutes,
            CreatedAt = l.CreatedAt
        };
    }
}
