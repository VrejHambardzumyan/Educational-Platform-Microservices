namespace CourseCatalogService.Infrastructure.Entities
{
    public class CourseRating
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string? Feedback { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
