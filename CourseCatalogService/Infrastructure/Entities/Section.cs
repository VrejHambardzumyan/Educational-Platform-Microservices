namespace CourseCatalogService.Infrastructure.Entities
{
    public class Section
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public required string Title { get; set; }
        public int Order { get; set; }

        public Course Course { get; set; } = null!;
        public ICollection<Lesson> Lessons { get; set; } = [];
    }
}
