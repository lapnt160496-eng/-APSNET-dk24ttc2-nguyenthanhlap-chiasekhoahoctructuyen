using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nguyenvanlap.Data;
using nguyenvanlap.Models;

namespace nguyenvanlap.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Courses
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Lessons)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return View(courses);
        }

        // GET: Admin/Courses/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Admin/Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Price,InstructorName,InstructorBio,CategoryId,Status")] Course course, IFormFile? coverImage)
        {
            // Remove validation for navigation properties
            ModelState.Remove("Category");
            
            if (ModelState.IsValid)
            {
                if (coverImage != null && coverImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "courses");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + coverImage.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await coverImage.CopyToAsync(fileStream);
                    }

                    course.CoverImage = "/uploads/courses/" + uniqueFileName;
                }

                course.CreatedAt = DateTime.Now;
                _context.Add(course);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Khóa học đã được tạo thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // GET: Admin/Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // POST: Admin/Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price,CoverImage,InstructorName,InstructorBio,CategoryId,Status,CreatedAt")] Course course, IFormFile? coverImage)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (coverImage != null && coverImage.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "courses");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + coverImage.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await coverImage.CopyToAsync(fileStream);
                        }

                        course.CoverImage = "/uploads/courses/" + uniqueFileName;
                    }

                    course.UpdatedAt = DateTime.Now;
                    _context.Update(course);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Khóa học đã được cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // GET: Admin/Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Admin/Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                // Check if there are any orders for this course
                var hasOrders = await _context.Orders.AnyAsync(o => o.CourseId == id);
                if (hasOrders)
                {
                    TempData["Error"] = "Không thể xóa khóa học này vì đã có học viên đăng ký (có đơn hàng). Hãy chuyển trạng thái sang 'Nháp' hoặc 'Ẩn' thay thế.";
                    return RedirectToAction(nameof(Index));
                }

                try 
                {
                    _context.Courses.Remove(course);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Khóa học đã được xóa thành công!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Đã xảy ra lỗi khi xóa khóa học: " + ex.Message;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
