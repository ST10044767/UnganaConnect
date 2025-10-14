using System.ComponentModel.DataAnnotations;
using UnganaConnect.Models.Users;

namespace UnganaConnect.Models.Training___Learning
{
    public class UserProgress
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public int ModuleId { get; set; }
        
        public bool IsCompleted { get; set; }
        
        public Module? Module { get; set; }
    }

    public class Certificate
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public int CourseId { get; set; }
        
        public DateTime IssuedOn { get; set; } = DateTime.UtcNow;
        
        [Url]
        public string? FileUrl { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Course? Course { get; set; }
    }
}
