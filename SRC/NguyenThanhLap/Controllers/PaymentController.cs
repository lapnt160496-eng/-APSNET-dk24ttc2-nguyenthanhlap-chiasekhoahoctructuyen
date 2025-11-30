using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nguyenthanhlap.Data;
using nguyenthanhlap.Models;

namespace nguyenthanhlap.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Checkout(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            // Check if already purchased
            var existingOrder = await _context.Orders
                .FirstOrDefaultAsync(o => o.UserId == userId && 
                                         o.CourseId == id && 
                                         o.PaymentStatus == PaymentStatus.Completed);

            if (existingOrder != null)
            {
                TempData["Info"] = "Bạn đã sở hữu khóa học này!";
                return RedirectToAction("Learn", "Courses", new { id });
            }

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(int courseId, string paymentMethod)
        {
            var userId = _userManager.GetUserId(User);

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }

            // Check if already purchased
            var existingOrder = await _context.Orders
                .FirstOrDefaultAsync(o => o.UserId == userId && 
                                         o.CourseId == courseId && 
                                         o.PaymentStatus == PaymentStatus.Completed);

            if (existingOrder != null)
            {
                TempData["Info"] = "Bạn đã sở hữu khóa học này!";
                return RedirectToAction("Learn", "Courses", new { id = courseId });
            }

            // Create order
            var order = new Order
            {
                UserId = userId!,
                CourseId = courseId,
                Amount = course.Price,
                PaymentMethod = paymentMethod,
                PaymentStatus = PaymentStatus.Pending,
                TransactionId = Guid.NewGuid().ToString()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Simulate payment processing
            // In a real application, you would integrate with payment gateways here
            // For demo purposes, we'll mark it as completed immediately

            if (paymentMethod == "Demo")
            {
                order.PaymentStatus = PaymentStatus.Completed;
                order.CompletedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thanh toán thành công! Bạn đã có thể học khóa học này.";
                return RedirectToAction("Learn", "Courses", new { id = courseId });
            }

            // For other payment methods, redirect to payment gateway
            // This is where you would integrate Stripe, PayPal, VNPay, etc.
            return RedirectToAction(nameof(PaymentGateway), new { orderId = order.Id, method = paymentMethod });
        }

        public async Task<IActionResult> PaymentGateway(int orderId, string method)
        {
            var order = await _context.Orders
                .Include(o => o.Course)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.PaymentMethod = method;
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            // In a real application, you would verify the payment with the gateway
            order.PaymentStatus = PaymentStatus.Completed;
            order.CompletedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thanh toán thành công! Bạn đã có thể học khóa học này.";
            return RedirectToAction("Learn", "Courses", new { id = order.CourseId });
        }

        public async Task<IActionResult> Success(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Course)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public IActionResult Failed()
        {
            return View();
        }
    }
}
