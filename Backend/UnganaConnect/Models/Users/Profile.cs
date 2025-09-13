namespace UnganaConnect.Models.Users
{
    public class Profile
    {
        public int Id { get; set; }

        // Foreign key → links Profile to User
        public int UserId { get; set; }
        public User User { get; set; }

        // Profile-specific fields
        public string Bio { get; set; }
        public string PhoneNumber { get; set; }
        public string Organization { get; set; }
        public string Country { get; set; }
        public string ProfilePictureUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
