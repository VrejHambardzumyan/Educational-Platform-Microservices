using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Application.Models.DTOs;
using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;

namespace CourseCatalogService.Application.Services
{
    public class SectionService(
        ISectionRepository sectionRepository,
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository) : ISectionService
    {
        private readonly ISectionRepository _sections = sectionRepository;
        private readonly ILessonRepository _lessons = lessonRepository;
        private readonly ICourseRepository _courses = courseRepository;

        public async Task<SectionResponseDto> CreateAsync(int courseId, SectionRequestDto dto, int instructorId, bool isAdmin = false, CancellationToken cancellationToken = default)
        {
            var course = await _courses.GetCourseByIdAsync(courseId, cancellationToken)
                ?? throw new KeyNotFoundException($"Course {courseId} not found.");

            if (!isAdmin && course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You do not own this course.");

            var section = new Section { CourseId = courseId, Title = dto.Title, Order = dto.Order };
            await _sections.AddAsync(section, cancellationToken);
            return ToDto(section);
        }

        public async Task<IEnumerable<SectionResponseDto>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default)
        {
            var sections = await _sections.GetByCourseIdAsync(courseId, cancellationToken);
            return sections.Select(ToDto);
        }

        public async Task<LessonResponseDto> AddLessonAsync(int sectionId, LessonRequestDto dto, int instructorId, bool isAdmin = false, CancellationToken cancellationToken = default)
        {
            var section = await _sections.GetByIdAsync(sectionId, cancellationToken)
                ?? throw new KeyNotFoundException($"Section {sectionId} not found.");

            var course = await _courses.GetCourseByIdAsync(section.CourseId, cancellationToken)
                ?? throw new KeyNotFoundException($"Course {section.CourseId} not found.");

            if (!isAdmin && course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You do not own this course.");

            var lesson = new Lesson
            {
                SectionId = sectionId,
                CourseId = section.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                OrderIndex = dto.OrderIndex,
                DurationInMinutes = dto.DurationInMinutes
            };

            await _lessons.AddAsync(lesson);
            return ToLessonDto(lesson);
        }

        public async Task<IEnumerable<LessonResponseDto>> GetLessonsAsync(int sectionId, CancellationToken cancellationToken = default)
        {
            _ = await _sections.GetByIdAsync(sectionId, cancellationToken)
                ?? throw new KeyNotFoundException($"Section {sectionId} not found.");

            var lessons = await _lessons.GetBySectionIdAsync(sectionId, cancellationToken);
            return lessons.Select(ToLessonDto);
        }

        private static SectionResponseDto ToDto(Section s) => new()
        {
            Id = s.Id,
            CourseId = s.CourseId,
            Title = s.Title,
            Order = s.Order
        };

        private static LessonResponseDto ToLessonDto(Lesson l) => new()
        {
            Id = l.Id,
            CourseId = l.CourseId,
            SectionId = l.SectionId,
            Title = l.Title,
            Description = l.Description,
            OrderIndex = l.OrderIndex,
            DurationInMinutes = l.DurationInMinutes,
            CreatedAt = l.CreatedAt
        };
    }
}
