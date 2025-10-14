using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.UI.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        
        // Navigation properties
        public User? User { get; set; }
        public Course? Course { get; set; }
    }
}
