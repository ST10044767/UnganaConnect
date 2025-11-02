using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnganaConnect.Models.Course
{
    public class Module
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string Content { get; set; } = string.Empty;

        public int Order { get; set; } = 0;

        [MaxLength(500)]
        public string VideoUrl { get; set; } = string.Empty; // Blob URL

        [MaxLength(255)]
        public string VideoFileName { get; set; } = string.Empty; // e.g., "lesson1.mp4"

        [MaxLength(100)]
        public string VideoContentType { get; set; } = string.Empty; // e.g., "video/mp4"

        public long VideoFileSize { get; set; } = 0; // in bytes

        // Foreign Key
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        // Relationship
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();

    }

}
