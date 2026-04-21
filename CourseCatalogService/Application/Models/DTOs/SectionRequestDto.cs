namespace CourseCatalogService.Application.Models.DTOs
{
    public class SectionRequestDto
    {
        public required string Title { get; init; }
        public int Order { get; init; }
    }
}
