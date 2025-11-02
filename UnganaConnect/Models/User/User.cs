using UnganaConnect.Models.Course;

namespace UnganaConnect.Models.User
{
        public class User
        {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Role { get; set; } = "Member";
        
        // Profile fields
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public string? Phone { get; set; }
        public string? Organization { get; set; }
        public string? Location { get; set; }
        public string? Website { get; set; }

        public ICollection<Module> Modules { get; set; } = new List<Module>();
        public ICollection<CourseEnrollment> Enrollments { get; set; } = new List<CourseEnrollment>();


    }
    }

