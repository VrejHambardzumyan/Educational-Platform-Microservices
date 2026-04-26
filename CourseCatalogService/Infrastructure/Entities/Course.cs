using NpgsqlTypes;

namespace CourseCatalogService.Infrastructure.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required int DurationInMonth { get; set; }
        public required int Price { get; set; }
        public int InstructorId { get; set; }
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
        public string? Category { get; set; }
        public string? ThumbnailUrl { get; set; }
        public bool IsDeleted { get; set; }
        public double AverageRating { get; set; } = 0;
        public int RatingCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public NpgsqlTsVector SearchVector { get; set; } = null!;
    }

    public enum CourseStatus
    {
        Draft = 0,
        Published = 1,
        Archived = 2
    }
}
