using System.ComponentModel.DataAnnotations;
using UnganaConnect.Models.Users;

namespace UnganaConnect.Models.Event_Management
{
    public class Event
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        public List<EventRegistration> EventRegistrations { get; set; } = new();
    }

    public class EventRegistration
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event Events { get; set; }
        public int UserId { get; set; }
        public User Users { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
