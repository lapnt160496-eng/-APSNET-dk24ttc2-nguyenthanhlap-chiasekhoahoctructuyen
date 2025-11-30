using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nguyenthanhlap.Models
{
    public enum CourseStatus
    {
        Draft,
        Published
    }

    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề khóa học là bắt buộc")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999999)]
        public decimal Price { get; set; }

        public string? CoverImage { get; set; }

        public string? InstructorName { get; set; }

        public string? InstructorBio { get; set; }

        public CourseStatus Status { get; set; } = CourseStatus.Draft;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Foreign keys
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }

        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<CourseProgress> CourseProgresses { get; set; } = new List<CourseProgress>();
    }
}
