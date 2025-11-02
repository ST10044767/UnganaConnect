using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnganaConnect.Models.Blog
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Content")]
        public string Content { get; set; } = string.Empty;
        
        [MaxLength(500)]
        [Display(Name = "Excerpt")]
        public string? Excerpt { get; set; }
        
        [MaxLength(500)]
        [Display(Name = "Featured Image URL")]
        public string? FeaturedImageUrl { get; set; }
        
        [Required]
        [Display(Name = "Category")]
        [MaxLength(50)]
        public string Category { get; set; } = "General";
        
        [Display(Name = "Tags")]
        public string? Tags { get; set; } // Comma-separated tags
        
        [Display(Name = "Status")]
        [MaxLength(20)]
        public string Status { get; set; } = "Draft"; // Draft, Published, Archived
        
        [Display(Name = "Views")]
        public int Views { get; set; } = 0;
        
        [Display(Name = "Is Featured")]
        public bool IsFeatured { get; set; } = false;
        
        [Display(Name = "Allow Comments")]
        public bool AllowComments { get; set; } = true;
        
        // Foreign Key to User (Author)
        [Required]
        [ForeignKey(nameof(Author))]
        public Guid AuthorId { get; set; }
        public UnganaConnect.Models.User.User Author { get; set; } = null!;
        
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "Published At")]
        public DateTime? PublishedAt { get; set; }
    }
}

