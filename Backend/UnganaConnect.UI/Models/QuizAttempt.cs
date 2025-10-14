using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.UI.Models
{
    public class QuizAttempt
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public int QuizId { get; set; }
        public int Score { get; set; } = 0;
        public int TotalQuestions { get; set; } = 0;
        public int CorrectAnswers { get; set; } = 0;
        public bool IsPassed { get; set; } = false;
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public int TimeSpent { get; set; } = 0; // in minutes
        
        // Navigation properties
        public User? User { get; set; }
        public Quiz? Quiz { get; set; }
        public List<QuizAnswer>? Answers { get; set; }
    }
}
