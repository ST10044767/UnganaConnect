using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.Models.Users
{
    public class Admin
    {
        public int Id { get; set; }
        
        [Required]
        public int AdminId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string ActionType { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public User? AdminUser { get; set; }
    }
}
