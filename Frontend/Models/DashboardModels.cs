namespace UnganaConnect.Frontend.Models
{
    public class DashboardStat
    {
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Change { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class RecentCourse
    {
        public string Title { get; set; } = string.Empty;
        public int Progress { get; set; }
        public string TimeLeft { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class UpcomingEvent
    {
        public string Title { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class DashboardViewModel
    {
        public List<DashboardStat> Stats { get; set; } = new();
        public List<RecentCourse> RecentCourses { get; set; } = new();
        public List<UpcomingEvent> UpcomingEvents { get; set; } = new();
    }
}