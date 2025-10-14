using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.UI.Models
{
    public class Option
    {
        public int Id { get; set; }
        
        [Required]
        public string OptionText { get; set; } = "";
        
        public bool IsCorrect { get; set; } = false;
        public int QuestionId { get; set; }
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Question? Question { get; set; }
    }
}
