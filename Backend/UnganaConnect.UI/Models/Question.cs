using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.UI.Models
{
    public class Question
    {
        public int Id { get; set; }
        
        [Required]
        public string QuestionText { get; set; } = "";
        
        public string? Explanation { get; set; }
        public int QuizId { get; set; }
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Quiz? Quiz { get; set; }
        public List<Option>? Options { get; set; }
    }
}
