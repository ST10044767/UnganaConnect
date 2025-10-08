using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.Frontend.Models
{
    public class CourseViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public string Instructor { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int Enrolled { get; set; }
        public int Progress { get; set; }
        public string Status { get; set; } = "available";
        public string Thumbnail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<ModuleViewModel> Modules { get; set; } = new();
    }

    public class ModuleViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Order { get; set; }
    }

    public class CourseCatalogViewModel
    {
        public List<CourseViewModel> Courses { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
        public string SelectedCategory { get; set; } = "all";
    }
}