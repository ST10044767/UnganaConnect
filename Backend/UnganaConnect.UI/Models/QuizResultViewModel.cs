using System.Collections.Generic;

namespace UnganaConnect.UI.Models
{
    public class QuizResultViewModel
    {
        public int QuizId { get; set; }
        public string QuizTitle { get; set; } = "";
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public bool IsPassed { get; set; }
        public int TimeSpent { get; set; }
        public List<QuizAnswerResult> AnswerResults { get; set; } = new List<QuizAnswerResult>();
    }

    public class QuizAnswerResult
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = "";
        public int? SelectedOptionId { get; set; }
        public int? CorrectOptionId { get; set; }
        public bool IsCorrect { get; set; }
        public string Explanation { get; set; } = "";
    }
}
