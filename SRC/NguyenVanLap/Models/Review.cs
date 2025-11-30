using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nguyenvanlap.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public int CourseId { get; set; }

        [Required(ErrorMessage = "Đánh giá là bắt buộc")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;
    }
}
