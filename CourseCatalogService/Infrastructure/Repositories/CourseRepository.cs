using Microsoft.EntityFrameworkCore;
using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;

namespace CourseCatalogService.Infrastructure.Repositories
{
    public class CourseRepository(CourseDbContext dbContext) : ICourseRepository
    {
        public readonly CourseDbContext _context = dbContext;


        public async Task<Course> AddEntityAsync(Course course)
        {
                
            //_context.Courses.Add(entity);

            //await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course?> GetCourseByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Courses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses.ToListAsync();
        }
    }
}
