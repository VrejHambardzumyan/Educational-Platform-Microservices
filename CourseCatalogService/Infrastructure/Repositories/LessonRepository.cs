using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseCatalogService.Infrastructure.Repositories
{
    public class LessonRepository(CourseDbContext context) : ILessonRepository
    {
        private readonly CourseDbContext _context = context;

        public async Task<Lesson> AddAsync(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
            return lesson;
        }

        public async Task<IEnumerable<Lesson>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Lessons
                .Where(l => l.CourseId == courseId && !l.IsDeleted)
                .OrderBy(l => l.OrderIndex)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Lesson?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Lessons
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, cancellationToken);
        }

        public async Task UpdateAsync(Lesson lesson)
        {
            lesson.UpdatedAt = DateTime.UtcNow;
            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return false;
            lesson.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountByCourseIdAsync(int courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Lessons.CountAsync(l => l.CourseId == courseId && !l.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<Lesson>> GetBySectionIdAsync(int sectionId, CancellationToken cancellationToken = default)
        {
            return await _context.Lessons
                .Where(l => l.SectionId == sectionId && !l.IsDeleted)
                .OrderBy(l => l.OrderIndex)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
