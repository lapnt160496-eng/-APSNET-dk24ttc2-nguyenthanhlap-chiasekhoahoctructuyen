using nguyenthanhlap.Models;

namespace nguyenthanhlap.ViewModels
{
    public class CourseDetailsViewModel
    {
        public Course Course { get; set; } = null!;
        public List<Lesson> Lessons { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public bool IsEnrolled { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }

    public class MyCourseViewModel
    {
        public Course Course { get; set; } = null!;
        public CourseProgress? Progress { get; set; }
        public int CompletionPercentage { get; set; }
        public int ProgressPercentage { get; set; }
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
    }

    public class LessonPlayerViewModel
    {
        public Course Course { get; set; } = null!;
        public Lesson CurrentLesson { get; set; } = null!;
        public List<Lesson> AllLessons { get; set; } = new();
        public CourseProgress? Progress { get; set; }
    }
}
