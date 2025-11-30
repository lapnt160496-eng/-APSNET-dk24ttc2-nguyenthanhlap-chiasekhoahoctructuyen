using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nguyenthanhlap.Models
{
    public class Lesson
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề bài học là bắt buộc")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? VideoUrl { get; set; }

        public string? DocumentUrl { get; set; }

        public int OrderIndex { get; set; }

        public int DurationMinutes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign keys
        public int CourseId { get; set; }

        // Navigation properties
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;
    }
}
