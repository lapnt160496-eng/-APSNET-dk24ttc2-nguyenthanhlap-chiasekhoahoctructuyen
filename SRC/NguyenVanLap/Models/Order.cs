using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nguyenvanlap.Models
{
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public int CourseId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public string? PaymentMethod { get; set; }

        public string? TransactionId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;
    }
}
