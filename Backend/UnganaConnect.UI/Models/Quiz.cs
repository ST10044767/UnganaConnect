using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.UI.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = "";
        
        [Required]
        public string Description { get; set; } = "";
        
        public int ModuleId { get; set; }
        public int TimeLimit { get; set; } // in minutes
        public int PassingScore { get; set; } // percentage
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Module? Module { get; set; }
        public List<Question>? Questions { get; set; }
    }
}
