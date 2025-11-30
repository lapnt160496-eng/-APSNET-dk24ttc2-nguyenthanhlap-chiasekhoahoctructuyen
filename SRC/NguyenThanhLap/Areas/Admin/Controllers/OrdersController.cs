using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nguyenthanhlap.Data;

namespace nguyenthanhlap.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index(string? status)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Course)
                    .ThenInclude(c => c.Category)
                .OrderByDescending(o => o.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<Models.PaymentStatus>(status, out var paymentStatus))
                {
                    query = query.Where(o => o.PaymentStatus == paymentStatus);
                }
            }

            var orders = await query.ToListAsync();
            ViewBag.Status = status;

            return View(orders);
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Course)
                    .ThenInclude(c => c.Category)
                .Include(o => o.Course)
                    .ThenInclude(c => c.Lessons)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
