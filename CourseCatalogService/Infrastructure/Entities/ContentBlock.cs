using System.ComponentModel.DataAnnotations.Schema;

namespace CourseCatalogService.Infrastructure.Entities
{
    public enum ContentBlockType { Text = 0, Video = 1, File = 2 }

    public class ContentBlock
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public ContentBlockType Type { get; set; }
        public int Order { get; set; }
        public string? TextContent { get; set; }
        public string? VideoUrl { get; set; }
        public string? FileUrl { get; set; }

        [Column(TypeName = "jsonb")]
        public string? Metadata { get; set; }

        public Lesson Lesson { get; set; } = null!;
    }
}
