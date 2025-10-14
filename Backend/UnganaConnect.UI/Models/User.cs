namespace UnganaConnect.UI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Role { get; set; } = "Member";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class Profile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Bio { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Organization { get; set; } = "";
        public string Country { get; set; } = "";
        public string ProfilePictureUrl { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User? User { get; set; }
    }
}
