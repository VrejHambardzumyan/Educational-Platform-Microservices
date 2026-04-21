using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseCatalogService.Infrastructure.Repositories
{
    public class SectionRepository(CourseDbContext context) : ISectionRepository
    {
        private readonly CourseDbContext _context = context;

        public async Task<Section> AddAsync(Section section, CancellationToken cancellationToken = default)
        {
            _context.Sections.Add(section);
            await _context.SaveChangesAsync(cancellationToken);
            return section;
        }

        public async Task<IEnumerable<Section>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Sections
                .Where(s => s.CourseId == courseId)
                .OrderBy(s => s.Order)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Section?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Sections
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(Section section, CancellationToken cancellationToken = default)
        {
            _context.Sections.Update(section);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Section section, CancellationToken cancellationToken = default)
        {
            _context.Sections.Remove(section);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
