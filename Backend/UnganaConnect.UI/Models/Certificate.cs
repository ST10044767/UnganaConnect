using System;
namespace UnganaConnect.UI.Models
{
    public class Certificate
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public int CourseId { get; set; }
        public DateTime IssuedOn { get; set; }
        public string FileUrl { get; set; } = "";

        // Navigation properties
        public User? User { get; set; }
        public Course? Course { get; set; }
    }
}
