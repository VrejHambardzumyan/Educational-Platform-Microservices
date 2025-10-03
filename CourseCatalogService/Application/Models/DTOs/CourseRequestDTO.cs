namespace CourseCatalogService.Application.Models.DTOs
{
    public class CourseRequestDto
    {
        public required string Title { get; init; }
        public required string Description { get; init; }
        public required int DurationInMonth { get; init; }
        public required int Price { get; init; }
    }
}
