using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.Models.Users
{
    public class Profile
    {
        public int Id { get; set; }

        // Foreign key → links Profile to User
        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }

        // Profile-specific fields
        [StringLength(500)]
        public string Bio { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Organization { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;
        
        [Url]
        public string ProfilePictureUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
