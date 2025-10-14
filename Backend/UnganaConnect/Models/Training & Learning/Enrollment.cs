using UnganaConnect.Models.Users;

namespace UnganaConnect.Models.Training___Learning
{
    public class Enrollment
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Course Course { get; set; } = null!;
        public int Count { get; set; }

     
    }
}