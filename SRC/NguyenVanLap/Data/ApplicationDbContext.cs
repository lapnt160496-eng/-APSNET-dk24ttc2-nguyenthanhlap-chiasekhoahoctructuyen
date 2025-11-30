using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using nguyenvanlap.Models;

namespace nguyenvanlap.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CourseProgress> CourseProgresses { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Course>()
                .HasOne(c => c.Category)
                .WithMany(cat => cat.Courses)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Lesson>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .HasOne(o => o.Course)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CourseProgress>()
                .HasOne(cp => cp.User)
                .WithMany(u => u.CourseProgresses)
                .HasForeignKey(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CourseProgress>()
                .HasOne(cp => cp.Course)
                .WithMany(c => c.CourseProgresses)
                .HasForeignKey(cp => cp.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.Course)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed initial data
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Lập trình", Description = "Các khóa học về lập trình và phát triển phần mềm", CreatedAt = new DateTime(2025, 1, 1) },
                new Category { Id = 2, Name = "Marketing", Description = "Các khóa học về marketing và quảng cáo", CreatedAt = new DateTime(2025, 1, 1) },
                new Category { Id = 3, Name = "Thiết kế", Description = "Các khóa học về thiết kế đồ họa và UI/UX", CreatedAt = new DateTime(2025, 1, 1) },
                new Category { Id = 4, Name = "Kinh doanh", Description = "Các khóa học về quản trị kinh doanh", CreatedAt = new DateTime(2025, 1, 1) },
                new Category { Id = 5, Name = "Ngoại ngữ", Description = "Các khóa học ngoại ngữ", CreatedAt = new DateTime(2025, 1, 1) }
            );
        }
    }
}
