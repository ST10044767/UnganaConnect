using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.UI.Models
{
    public class Course
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = "";
        
        [Required]
        public string Description { get; set; } = "";
        
        [Required]
        [StringLength(100)]
        public string Category { get; set; } = "";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public List<Module> Modules { get; set; } = new();
        public List<Enrollment> Enrollments { get; set; } = new();
    }
}
