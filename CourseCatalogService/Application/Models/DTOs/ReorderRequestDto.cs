namespace CourseCatalogService.Application.Models.DTOs
{
    public class ReorderRequestDto
    {
        public required List<int> BlockIds { get; init; }
    }
}
