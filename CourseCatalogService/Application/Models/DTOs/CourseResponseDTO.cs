namespace CourseCatalogService.Application.Models.DTOs
{
    public class CourseResponseDto
    {
        public int Id { get; init; }
        public required string Title { get; init; }
        public required int DurationInMonth { get; init; }
        public required int Price { get; init; }
    }
}
