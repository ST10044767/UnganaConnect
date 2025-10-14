using System.ComponentModel.DataAnnotations;
using UnganaConnect.Models.Training___Learning;

namespace UnganaConnect.Models.Resources_Repo
{
    public class CResource
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Url]
        public string FileUrl { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
        
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        // Foreign Key to Course
        [Required]
        public int CourseId { get; set; }
        
        public Course? Course { get; set; }

        public long FileSize { get; set; } // in bytes
        
        [Required]
        [StringLength(100)]
        public string FileType { get; set; } = string.Empty; // e.g., pdf, docx, mp4
        
        [Required]
        public string FilePath { get; set; } = string.Empty;
    }
}