using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nguyenvanlap.Data;
using nguyenvanlap.Models;

namespace nguyenvanlap.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var featuredCourses = await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Reviews)
            .Where(c => c.Status == CourseStatus.Published)
            .OrderByDescending(c => c.CreatedAt)
            .Take(6)
            .ToListAsync();

        var categories = await _context.Categories
            .Include(c => c.Courses)
            .ToListAsync();

        ViewBag.Categories = categories;

        return View(featuredCourses);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

