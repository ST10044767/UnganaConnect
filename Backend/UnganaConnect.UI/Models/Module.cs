using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.UI.Models
{
    public class Module
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = "";
        
        [Required]
        public string Content { get; set; } = "";
        
        public string? VideoUrl { get; set; }
        public string? FileUrl { get; set; }
        public int CourseId { get; set; }
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Course? Course { get; set; }
        public List<Quiz>? Quizzes { get; set; }
    }
}
