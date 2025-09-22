namespace UnganaConnect.Models.Training___Learning
{
    public class UserProgress
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public int ModuleId { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class Certificate
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public int CourseId { get; set; }
        public DateTime IssuedOn { get; set; } = DateTime.UtcNow;
    }

}
