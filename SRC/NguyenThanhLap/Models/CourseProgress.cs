using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nguyenvanlap.Models
{
    public class CourseProgress
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public int CourseId { get; set; }

        public int? LastLessonId { get; set; }

        public int CompletedLessons { get; set; } = 0;

        public int TotalLessons { get; set; }

        public DateTime LastAccessedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;
    }
}
