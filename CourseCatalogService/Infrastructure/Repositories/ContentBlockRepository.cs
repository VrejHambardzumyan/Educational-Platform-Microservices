using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseCatalogService.Infrastructure.Repositories
{
    public class ContentBlockRepository(CourseDbContext context) : IContentBlockRepository
    {
        private readonly CourseDbContext _context = context;

        public async Task<ContentBlock> AddAsync(ContentBlock block, CancellationToken cancellationToken = default)
        {
            _context.ContentBlocks.Add(block);
            await _context.SaveChangesAsync(cancellationToken);
            return block;
        }

        public async Task<ContentBlock?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ContentBlocks
                .FirstOrDefaultAsync(cb => cb.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<ContentBlock>> GetByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default)
        {
            return await _context.ContentBlocks
                .Where(cb => cb.LessonId == lessonId)
                .OrderBy(cb => cb.Order)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(ContentBlock block, CancellationToken cancellationToken = default)
        {
            _context.ContentBlocks.Update(block);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(ContentBlock block, CancellationToken cancellationToken = default)
        {
            _context.ContentBlocks.Remove(block);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateRangeAsync(IEnumerable<ContentBlock> blocks, CancellationToken cancellationToken = default)
        {
            _context.ContentBlocks.UpdateRange(blocks);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
