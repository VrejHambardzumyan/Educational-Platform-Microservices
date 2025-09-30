namespace CourseCatalogService.Infrastructure.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required int DurationInMonth { get; set; }
        public required int Price { get; set; }

    }
}
