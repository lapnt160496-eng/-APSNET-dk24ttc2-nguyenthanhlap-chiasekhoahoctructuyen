using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using nguyenthanhlap.Models;

namespace nguyenthanhlap.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Xóa dữ liệu cũ (Clear existing data)
            if (context.Reviews.Any()) { context.Reviews.RemoveRange(context.Reviews); }
            if (context.CourseProgresses.Any()) { context.CourseProgresses.RemoveRange(context.CourseProgresses); }
            if (context.Orders.Any()) { context.Orders.RemoveRange(context.Orders); }
            if (context.Lessons.Any()) { context.Lessons.RemoveRange(context.Lessons); }
            if (context.Courses.Any()) { context.Courses.RemoveRange(context.Courses); }
            
            // Xóa các user mẫu cũ
            var sampleUsers = await userManager.Users.Where(u => u.Email.StartsWith("user") && u.Email.EndsWith("@example.com")).ToListAsync();
            foreach (var user in sampleUsers)
            {
                await userManager.DeleteAsync(user);
            }
            
            await context.SaveChangesAsync();

            // 2. Tạo dữ liệu mới (Create new data - 5 items each)

            // Tạo 5 User
            var users = new List<ApplicationUser>();
            for (int i = 1; i <= 5; i++)
            {
                var email = $"user{i}@example.com";
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = $"Học Viên {i}",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-i)
                };

                var result = await userManager.CreateAsync(user, "User@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                    users.Add(user);
                }
            }

            // Đảm bảo có danh mục
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Lập trình", Description = "Các khóa học lập trình" },
                    new Category { Name = "Marketing", Description = "Các khóa học Marketing" },
                    new Category { Name = "Thiết kế", Description = "Các khóa học Thiết kế" },
                    new Category { Name = "Kinh doanh", Description = "Các khóa học Kinh doanh" },
                    new Category { Name = "Ngoại ngữ", Description = "Các khóa học Ngoại ngữ" }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }
            
            var categoriesList = await context.Categories.ToListAsync();

            // Tạo 5 Khóa học
            var courses = new List<Course>();
            var courseData = new[] 
            { 
                new { Title = "Lập trình Web Fullstack", CatIdx = 0, Price = 1500000m }, 
                new { Title = "Digital Marketing Cơ Bản", CatIdx = 1, Price = 800000m }, 
                new { Title = "Thiết kế UI/UX với Figma", CatIdx = 2, Price = 1200000m }, 
                new { Title = "Quản trị Kinh doanh 4.0", CatIdx = 3, Price = 2000000m }, 
                new { Title = "Tiếng Anh Giao Tiếp", CatIdx = 4, Price = 900000m } 
            };

            for (int i = 0; i < 5; i++)
            {
                var data = courseData[i];
                var category = categoriesList[data.CatIdx % categoriesList.Count];
                courses.Add(new Course
                {
                    Title = data.Title,
                    Description = $"Khóa học {data.Title} cung cấp kiến thức nền tảng và chuyên sâu, giúp bạn làm chủ kỹ năng trong thời gian ngắn nhất.",
                    Price = data.Price,
                    CategoryId = category.Id,
                    InstructorName = $"Giảng viên {i + 1}",
                    InstructorBio = "Chuyên gia hàng đầu với nhiều năm kinh nghiệm thực chiến.",
                    Status = CourseStatus.Published,
                    CreatedAt = DateTime.Now.AddDays(-i * 2),
                    CoverImage = null // Để null để dùng ảnh mặc định hoặc placeholder
                });
            }
            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();

            // Tạo Bài học (5 bài mỗi khóa)
            var lessons = new List<Lesson>();
            foreach (var course in courses)
            {
                for (int i = 1; i <= 5; i++)
                {
                    lessons.Add(new Lesson
                    {
                        CourseId = course.Id,
                        Title = $"Bài học {i}: Kiến thức phần {i}",
                        Description = $"Nội dung chi tiết của bài học số {i}. Hướng dẫn thực hành và bài tập áp dụng.",
                        VideoUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ", // Video demo
                        OrderIndex = i,
                        DurationMinutes = 15 + i * 5,
                        CreatedAt = DateTime.Now
                    });
                }
            }
            context.Lessons.AddRange(lessons);
            await context.SaveChangesAsync();

            // Tạo Đơn hàng (5 đơn)
            if (users.Any())
            {
                var orders = new List<Order>();
                for (int i = 0; i < 5; i++)
                {
                    var user = users[i % users.Count]; // Lấy user tương ứng
                    var course = courses[i % courses.Count]; // Lấy course tương ứng
                    
                    orders.Add(new Order
                    {
                        UserId = user.Id,
                        CourseId = course.Id,
                        Amount = course.Price,
                        PaymentStatus = PaymentStatus.Completed,
                        PaymentMethod = "Demo",
                        TransactionId = Guid.NewGuid().ToString(),
                        CreatedAt = DateTime.Now.AddDays(-1),
                        CompletedAt = DateTime.Now
                    });
                }
                context.Orders.AddRange(orders);
                await context.SaveChangesAsync();

                // Tạo Đánh giá (5 đánh giá)
                var reviews = new List<Review>();
                for (int i = 0; i < 5; i++)
                {
                    var user = users[i % users.Count];
                    var course = courses[i % courses.Count];
                    
                    reviews.Add(new Review
                    {
                        UserId = user.Id,
                        CourseId = course.Id,
                        Rating = 5,
                        Comment = "Khóa học rất chất lượng, giảng viên nhiệt tình!",
                        CreatedAt = DateTime.Now
                    });
                }
                context.Reviews.AddRange(reviews);
                await context.SaveChangesAsync();
            }
        }
    }
}
