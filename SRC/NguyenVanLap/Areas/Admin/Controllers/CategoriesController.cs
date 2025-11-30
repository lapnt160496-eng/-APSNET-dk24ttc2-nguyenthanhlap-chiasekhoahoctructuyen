using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nguyenvanlap.Data;
using nguyenvanlap.Models;

namespace nguyenvanlap.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Courses)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(categories);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.CreatedAt = DateTime.Now;
                _context.Add(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Danh mục đã được tạo thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,CreatedAt")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Danh mục đã được cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Courses)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Courses)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category != null)
            {
                if (category.Courses.Any())
                {
                    TempData["Error"] = "Không thể xóa danh mục này vì đang có khóa học sử dụng!";
                    return RedirectToAction(nameof(Index));
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Danh mục đã được xóa thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
