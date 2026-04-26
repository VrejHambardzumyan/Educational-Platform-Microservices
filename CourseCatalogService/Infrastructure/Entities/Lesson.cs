namespace CourseCatalogService.Infrastructure.Entities
{
    public class Lesson
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int OrderIndex { get; set; }
        public int DurationInMinutes { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int? SectionId { get; set; }

        public Course Course { get; set; } = null!;
        public Section? Section { get; set; }
        public ICollection<ContentBlock> ContentBlocks { get; set; } = [];
    }
}
