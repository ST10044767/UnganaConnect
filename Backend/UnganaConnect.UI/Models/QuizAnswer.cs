using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.UI.Models
{
    public class QuizAnswer
    {
        public int Id { get; set; }
        public int QuizAttemptId { get; set; }
        public int QuestionId { get; set; }
        public int? SelectedOptionId { get; set; }
        public bool IsCorrect { get; set; } = false;
        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public QuizAttempt? QuizAttempt { get; set; }
        public Question? Question { get; set; }
        public Option? SelectedOption { get; set; }
    }
}
