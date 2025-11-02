using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnganaConnect.Models.Course
{
    // QUIZ MODEL
    public class Quiz
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Instructions { get; set; } = string.Empty;

        // Foreign Key
        [ForeignKey("Module")]
        public int ModuleId { get; set; }
        public Module Module { get; set; }

        // Relationship: Questions of the quiz
        public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
    }

  
    // QUIZ QUESTION MODEL
    public class QuizQuestion
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(1000)]
        public string QuestionText { get; set; } = string.Empty;

        [MaxLength(50)]
        public string QuestionType { get; set; } = "multiple-choice"; // can be "true-false", "text"

        // Foreign Key
        [ForeignKey("Quiz")]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        // Relationship: Options for this question
        public ICollection<QuizOption> Options { get; set; } = new List<QuizOption>();
    }

    // QUIZ OPTION MODEL
    public class QuizOption
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public string Text { get; set; } = string.Empty;

        public bool IsCorrect { get; set; } = false;

        // Foreign Key
        [ForeignKey("QuizQuestion")]
        public int QuestionId { get; set; }
        public QuizQuestion Question { get; set; }
    }

    // QUIZ ATTEMPT MODEL
    public class QuizAttempt
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Quiz")]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        [Required]
        public Guid UserId { get; set; } // matches your User.Id

        public double Score { get; set; } = 0.0;
        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    }
}
