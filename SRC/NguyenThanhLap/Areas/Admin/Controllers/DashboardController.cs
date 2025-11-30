using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nguyenthanhlap.Data;
using nguyenthanhlap.Models;
using nguyenthanhlap.ViewModels;

namespace nguyenthanhlap.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalCourses = await _context.Courses.CountAsync(),
                TotalOrders = await _context.Orders.Where(o => o.PaymentStatus == PaymentStatus.Completed).CountAsync(),
                TotalRevenue = await _context.Orders.Where(o => o.PaymentStatus == PaymentStatus.Completed).SumAsync(o => o.Amount),
                RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Course)
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(10)
                    .ToListAsync(),
                RecentCourses = await _context.Courses
                    .Include(c => c.Category)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}
