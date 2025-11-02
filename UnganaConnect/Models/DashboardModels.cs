using UnganaConnect.Models.Course;

namespace UnganaConnect.Frontend.Models
{
   

    public class RecentCourse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Progress { get; set; }
        public string TimeLeft { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public int CourseId { get; set; }

        // Navigation property
        public Course Course { get; set; }


    }

    public class EnrolledCourse
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty; // Blob URL

    }

    public class UpcomingEvent
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class DashboardStats
    {
        public int CoursesCompleted { get; set; }
        public int CertificatesEarned { get; set; }
        public int ResourcesDownloaded { get; set; }
        public int EventsAttended { get; set; }
        public int TotalCourses { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalResources { get; set; }
        public int UpcomingEventsCount { get; set; }
    }

    public class DashboardViewModel
    {
        public List<RecentCourse> RecentCourses { get; set; } = new();
        public List<EnrolledCourse> EnrolledCourses { get; set; } = new();
        public List<UpcomingEvent> UpcomingEvents { get; set; } = new();
        public DashboardStats Stats { get; set; } = new();
    }

}