namespace UnganaConnect.Frontend.Models
{
    public class CourseModule
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
    }

    public class CourseContentViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public int Progress { get; set; }
        public List<CourseModule> Modules { get; set; } = new();
    }
}