using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nguyenthanhlap.Data;
using nguyenthanhlap.Models;

namespace nguyenthanhlap.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class LessonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LessonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Lessons?courseId=5
        public async Task<IActionResult> Index(int? courseId)
        {
            if (courseId == null)
            {
                // If no courseId is provided, list all lessons ordered by Course and then OrderIndex
                var allLessons = await _context.Lessons
                    .Include(l => l.Course)
                    .OrderBy(l => l.Course.Title)
                    .ThenBy(l => l.OrderIndex)
                    .ToListAsync();
                
                ViewBag.CourseId = null;
                ViewBag.CourseName = null;
                return View(allLessons);
            }

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Course = course;
            ViewBag.CourseId = course.Id;
            ViewBag.CourseName = course.Title;

            var lessons = await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .Include(l => l.Course)
                .OrderBy(l => l.OrderIndex)
                .ToListAsync();

            return View(lessons);
        }

        // GET: Admin/Lessons/Create?courseId=5
        public async Task<IActionResult> Create(int? courseId)
        {
            if (courseId == null)
            {
                return BadRequest();
            }

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Course = course;

            var maxOrder = await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .MaxAsync(l => (int?)l.OrderIndex) ?? 0;

            var lesson = new Lesson
            {
                CourseId = courseId.Value,
                OrderIndex = maxOrder + 1
            };

            return View(lesson);
        }

        // POST: Admin/Lessons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,VideoUrl,DocumentUrl,OrderIndex,DurationMinutes,CourseId")] Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                lesson.CreatedAt = DateTime.Now;
                _context.Add(lesson);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Bài học đã được tạo thành công!";
                return RedirectToAction(nameof(Index), new { courseId = lesson.CourseId });
            }

            var course = await _context.Courses.FindAsync(lesson.CourseId);
            ViewBag.Course = course;
            return View(lesson);
        }

        // GET: Admin/Lessons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
            {
                return NotFound();
            }

            ViewBag.Course = lesson.Course;
            return View(lesson);
        }

        // POST: Admin/Lessons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,VideoUrl,DocumentUrl,OrderIndex,DurationMinutes,CourseId,CreatedAt")] Lesson lesson)
        {
            if (id != lesson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lesson);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Bài học đã được cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonExists(lesson.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { courseId = lesson.CourseId });
            }

            var course = await _context.Courses.FindAsync(lesson.CourseId);
            ViewBag.Course = course;
            return View(lesson);
        }

        // GET: Admin/Lessons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // POST: Admin/Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                var courseId = lesson.CourseId;
                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Bài học đã được xóa thành công!";
                return RedirectToAction(nameof(Index), new { courseId });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LessonExists(int id)
        {
            return _context.Lessons.Any(e => e.Id == id);
        }
    }
}
