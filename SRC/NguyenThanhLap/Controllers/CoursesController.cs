using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nguyenthanhlap.Data;
using nguyenthanhlap.Models;
using nguyenthanhlap.ViewModels;

namespace nguyenthanhlap.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CoursesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Courses
        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            var query = _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Reviews)
                .Where(c => c.Status == CourseStatus.Published);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Title.Contains(search) || 
                                        c.Description.Contains(search) ||
                                        (c.InstructorName != null && c.InstructorName.Contains(search)));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == categoryId.Value);
            }

            var courses = await query.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;

            return View(courses);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Lessons)
                .Include(c => c.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            var isEnrolled = false;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                isEnrolled = await _context.Orders
                    .AnyAsync(o => o.UserId == userId && 
                                  o.CourseId == id && 
                                  o.PaymentStatus == PaymentStatus.Completed);
            }

            var viewModel = new CourseDetailsViewModel
            {
                Course = course,
                Lessons = course.Lessons.OrderBy(l => l.OrderIndex).ToList(),
                Reviews = course.Reviews.OrderByDescending(r => r.CreatedAt).ToList(),
                IsEnrolled = isEnrolled,
                AverageRating = course.Reviews.Any() ? course.Reviews.Average(r => r.Rating) : 0,
                TotalReviews = course.Reviews.Count
            };

            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> MyCourses()
        {
            var userId = _userManager.GetUserId(User);

            var enrolledCourses = await _context.Orders
                .Include(o => o.Course)
                    .ThenInclude(c => c.Lessons)
                .Include(o => o.Course.Category)
                .Where(o => o.UserId == userId && o.PaymentStatus == PaymentStatus.Completed)
                .Select(o => o.Course)
                .Distinct()
                .ToListAsync();

            var viewModels = new List<MyCourseViewModel>();

            foreach (var course in enrolledCourses)
            {
                var progress = await _context.CourseProgresses
                    .FirstOrDefaultAsync(cp => cp.UserId == userId && cp.CourseId == course.Id);

                var totalLessons = course.Lessons.Count;
                var completionPercentage = totalLessons > 0 && progress != null
                    ? (int)((double)progress.CompletedLessons / totalLessons * 100)
                    : 0;

                viewModels.Add(new MyCourseViewModel
                {
                    Course = course,
                    Progress = progress,
                    CompletionPercentage = completionPercentage
                });
            }

            return View(viewModels);
        }

        [Authorize]
        public async Task<IActionResult> Learn(int? id, int? lessonId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            // Check if user has purchased the course
            var hasAccess = await _context.Orders
                .AnyAsync(o => o.UserId == userId && 
                              o.CourseId == id && 
                              o.PaymentStatus == PaymentStatus.Completed);

            if (!hasAccess)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            var course = await _context.Courses
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            var lessons = course.Lessons.OrderBy(l => l.OrderIndex).ToList();

            Lesson? currentLesson;
            if (lessonId.HasValue)
            {
                currentLesson = lessons.FirstOrDefault(l => l.Id == lessonId);
            }
            else
            {
                // Get the last accessed lesson or the first lesson
                var progress = await _context.CourseProgresses
                    .FirstOrDefaultAsync(cp => cp.UserId == userId && cp.CourseId == id);

                if (progress?.LastLessonId != null)
                {
                    currentLesson = lessons.FirstOrDefault(l => l.Id == progress.LastLessonId);
                }
                else
                {
                    currentLesson = lessons.FirstOrDefault();
                }
            }

            if (currentLesson == null)
            {
                return NotFound();
            }

            // Update progress
            var courseProgress = await _context.CourseProgresses
                .FirstOrDefaultAsync(cp => cp.UserId == userId && cp.CourseId == id);

            if (courseProgress == null)
            {
                courseProgress = new CourseProgress
                {
                    UserId = userId!,
                    CourseId = id.Value,
                    LastLessonId = currentLesson.Id,
                    TotalLessons = lessons.Count,
                    CompletedLessons = 0
                };
                _context.CourseProgresses.Add(courseProgress);
            }
            else
            {
                courseProgress.LastLessonId = currentLesson.Id;
                courseProgress.LastAccessedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            var viewModel = new LessonPlayerViewModel
            {
                Course = course,
                CurrentLesson = currentLesson,
                AllLessons = lessons,
                Progress = courseProgress
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MarkLessonComplete(int lessonId)
        {
            var userId = _userManager.GetUserId(User);

            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null)
            {
                return NotFound();
            }

            var progress = await _context.CourseProgresses
                .FirstOrDefaultAsync(cp => cp.UserId == userId && cp.CourseId == lesson.CourseId);

            if (progress != null)
            {
                progress.CompletedLessons++;
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview(int courseId, int rating, string? comment)
        {
            var userId = _userManager.GetUserId(User);

            // Check if user has purchased the course
            var hasAccess = await _context.Orders
                .AnyAsync(o => o.UserId == userId && 
                              o.CourseId == courseId && 
                              o.PaymentStatus == PaymentStatus.Completed);

            if (!hasAccess)
            {
                return Forbid();
            }

            // Check if user already reviewed
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == courseId);

            if (existingReview != null)
            {
                existingReview.Rating = rating;
                existingReview.Comment = comment;
                existingReview.CreatedAt = DateTime.Now;
            }
            else
            {
                var review = new Review
                {
                    UserId = userId!,
                    CourseId = courseId,
                    Rating = rating,
                    Comment = comment
                };
                _context.Reviews.Add(review);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đánh giá của bạn đã được gửi!";
            return RedirectToAction(nameof(Details), new { id = courseId });
        }

        [Authorize]
        public async Task<IActionResult> TransactionHistory()
        {
            var userId = _userManager.GetUserId(User);

            var orders = await _context.Orders
                .Include(o => o.Course)
                    .ThenInclude(c => c.Category)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }
    }
}
