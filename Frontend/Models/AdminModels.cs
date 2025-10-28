using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.Frontend.Models
{
    public class AddVideoViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public string VideoUrl { get; set; } = string.Empty;
        
        public string Category { get; set; } = string.Empty;
        
        public string Duration { get; set; } = string.Empty;
        
        public string Level { get; set; } = string.Empty;
    }

    public class AdminUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string LastLogin { get; set; } = string.Empty;
    }

    public class AdminAnalytics
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalCourses { get; set; }
        public int CompletedCourses { get; set; }
        public int TotalResources { get; set; }
        public int ResourceDownloads { get; set; }
    }


}