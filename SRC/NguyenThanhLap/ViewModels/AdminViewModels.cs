using nguyenthanhlap.Models;

namespace nguyenthanhlap.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Order> RecentOrders { get; set; } = new();
        public List<Course> RecentCourses { get; set; } = new();
        public List<ApplicationUser> RecentUsers { get; set; } = new();
    }

    // Alias for backward compatibility
    public class AdminDashboardViewModel : DashboardViewModel { }
}
