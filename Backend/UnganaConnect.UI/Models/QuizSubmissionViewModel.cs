using System.Collections.Generic;

namespace UnganaConnect.UI.Models
{
    public class QuizSubmissionViewModel
    {
        public int QuizId { get; set; }
        public List<QuizAnswerViewModel> Answers { get; set; } = new List<QuizAnswerViewModel>();
    }

    public class QuizAnswerViewModel
    {
        public int QuestionId { get; set; }
        public int? SelectedOptionId { get; set; }
    }
}
