namespace UnganaConnect.Models.Training___Learning
{
    public class Enrollment
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";  
        public int CourseId { get; set; }
        public Course? Course { get; set; }
        public DateTime EnrolledOn { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
