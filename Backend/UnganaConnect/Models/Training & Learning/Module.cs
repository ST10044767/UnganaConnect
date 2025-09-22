namespace UnganaConnect.Models.Training___Learning
{
    public class Module
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = ""; 
        public int CourseId { get; set; }
        public Course? Course { get; set; }
        public string? VideoUrl { get; set; }   
        public string? FileUrl { get; set; }   
    }
}
