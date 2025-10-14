using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace UnganaConnect.Models.Training___Learning
{
    public class Course
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        public List<Module> Modules { get; set; } = new();
        public List<Enrollment> Enrollments { get; set; } = new();
    }
}
