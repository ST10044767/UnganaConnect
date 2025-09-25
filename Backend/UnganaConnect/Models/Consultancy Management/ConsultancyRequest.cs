using System.ComponentModel.DataAnnotations;
using UnganaConnect.Models.Users;

namespace UnganaConnect.Models.Consultancy_Management
{
    public class ConsultancyRequest
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Assigned, InProgress, Completed

        public int UserId { get; set; } // CSO who requested

        public int? AssignedStaffId { get; set; } // Staff assigned

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public User User { get; set; }
        public User AssignedStaff { get; set; }
    }
}
