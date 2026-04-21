namespace CourseCatalogService.Application.Models.DTOs
{
    public class SectionResponseDto
    {
        public int Id { get; init; }
        public int CourseId { get; init; }
        public required string Title { get; init; }
        public int Order { get; init; }
    }
}
