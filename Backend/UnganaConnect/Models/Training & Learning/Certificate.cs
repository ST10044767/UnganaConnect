namespace UnganaConnect.Models.Training___Learning
{
    public class Certificate
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime IssuedAt { get; set; }
    }
}
