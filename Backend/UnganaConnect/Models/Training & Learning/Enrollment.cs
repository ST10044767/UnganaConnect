namespace UnganaConnect.Models.Training___Learning
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public bool isCompleted { get; set; }
    }
}
