using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.Models.Course
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Duration { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Level { get; set; } = string.Empty;

        public double Rating { get; set; } = 0.0;
        public int Enrolled { get; set; } = 0;
        public int Progress { get; set; } = 0;

        [MaxLength(50)]
        public string Status { get; set; } = "available";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        [MaxLength(500)]
        public string ThumbnailUrl { get; set; } = string.Empty; // Blob URL

        [MaxLength(255)]
        public string ThumbnailFileName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ThumbnailContentType { get; set; } = string.Empty;

        // Relationship
        public ICollection<Module> Modules { get; set; } = new List<Module>();
    }


}
