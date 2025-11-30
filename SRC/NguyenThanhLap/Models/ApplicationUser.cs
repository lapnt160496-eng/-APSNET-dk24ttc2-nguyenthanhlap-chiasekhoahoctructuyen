using Microsoft.AspNetCore.Identity;

namespace nguyenvanlap.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<CourseProgress> CourseProgresses { get; set; } = new List<CourseProgress>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
