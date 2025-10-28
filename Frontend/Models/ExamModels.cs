namespace UnganaConnect.Frontend.Models
{
    public class ExamQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string[] Options { get; set; } = Array.Empty<string>();
        public string CorrectAnswer { get; set; } = string.Empty;
    }

    public class ExamViewModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<ExamQuestion> Questions { get; set; } = new();
    }
}