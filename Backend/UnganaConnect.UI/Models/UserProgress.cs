using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.UI.Models
{
    public class UserProgress
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public int ModuleId { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public int TimeSpent { get; set; } = 0; // in minutes
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public User? User { get; set; }
        public Module? Module { get; set; }
    }
}
