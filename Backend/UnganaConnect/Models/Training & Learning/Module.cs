using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.Models.Training___Learning
{
    public class Module
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        public int CourseId { get; set; }
        
        public Course? Course { get; set; }
        
        [Url]
        public string? VideoUrl { get; set; }
        
        [Url]
        public string? FileUrl { get; set; }
    }
}
