using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nguyenvanlap.Models;

namespace nguyenvanlap.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly nguyenvanlap.Data.ApplicationDbContext _context;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, nguyenvanlap.Data.ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<(ApplicationUser User, IList<string> Roles)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add((user, roles));
            }

            return View(userViewModels);
        }

        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;

            return View(user);
        }

        // POST: Admin/Users/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = $"Tài khoản đã được {(user.IsActive ? "kích hoạt" : "vô hiệu hóa")} thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Users/ChangeRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string id, string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!string.IsNullOrEmpty(role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            TempData["Success"] = "Vai trò đã được cập nhật thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser?.Id == user.Id)
                {
                    TempData["Error"] = "Bạn không thể xóa tài khoản của chính mình!";
                    return RedirectToAction(nameof(Index));
                }

                // Check for related data (Orders) - Restrict Delete
                var hasOrders = await _context.Orders.AnyAsync(o => o.UserId == id);
                if (hasOrders)
                {
                    TempData["Error"] = "Không thể xóa người dùng này vì họ đã có đơn hàng. Hãy vô hiệu hóa tài khoản thay thế.";
                    return RedirectToAction(nameof(Index));
                }

                // Check for related data (Reviews) - Delete reviews before deleting user
                var userReviews = await _context.Reviews.Where(r => r.UserId == id).ToListAsync();
                if (userReviews.Any())
                {
                    _context.Reviews.RemoveRange(userReviews);
                    await _context.SaveChangesAsync();
                }

                try
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = "Người dùng đã được xóa thành công!";
                    }
                    else
                    {
                        TempData["Error"] = "Lỗi khi xóa người dùng: " + string.Join(", ", result.Errors.Select(e => e.Description));
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Đã xảy ra lỗi hệ thống khi xóa người dùng: " + ex.Message;
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
