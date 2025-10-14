using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.Models.Training___Learning
{
    public class Quiz
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public int ModuleId { get; set; }
        
        public Module? Module { get; set; }
        public List<Question> Questions { get; set; } = new();
    }

    public class Question
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Text { get; set; } = string.Empty;
        
        public List<Option> Options { get; set; } = new();
        
        [Required]
        public int CorrectOptionId { get; set; }
        
        [Required]
        public int QuizId { get; set; }
        
        public Quiz? Quiz { get; set; }
    }

    public class Option
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Text { get; set; } = string.Empty;
        
        [Required]
        public int QuestionId { get; set; }
        
        public Question? Question { get; set; }
    }
}

