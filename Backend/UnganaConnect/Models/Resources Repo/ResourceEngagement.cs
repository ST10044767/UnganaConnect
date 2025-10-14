using System.ComponentModel.DataAnnotations;
using UnganaConnect.Models.Users;

namespace UnganaConnect.Models.Resources_Repo
{
    public class ResourceEngagement
    {
        public int Id { get; set; }
        
        [Required]
        public int ResourceId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        public DateTime DownloadedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public CResource? Resource { get; set; }
        public User? User { get; set; }
    }
}
