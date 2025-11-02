using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.Models.User
{
    public class ProfileViewModel
    {
        public Guid Id { get; set; }
        
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }
        
        [Display(Name = "Organization")]
        public string? Organization { get; set; }
        
        [Display(Name = "Location")]
        public string? Location { get; set; }
        
        [Display(Name = "Website")]
        [Url]
        public string? Website { get; set; }
        
        [Display(Name = "Bio")]
        [MaxLength(1000)]
        public string? Bio { get; set; }
        
        [Display(Name = "Profile Picture")]
        public string? ProfilePictureUrl { get; set; }
        
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

