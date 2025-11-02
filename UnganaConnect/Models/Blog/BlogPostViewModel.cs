using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.Models.Blog
{
    public class BlogPostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Excerpt { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public string Category { get; set; } = "General";
        public string? Tags { get; set; }
        public string Status { get; set; } = "Draft";
        public int Views { get; set; }
        public bool IsFeatured { get; set; }
        public bool AllowComments { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
    
    public class CreateBlogPostViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Content is required.")]
        [Display(Name = "Content")]
        public string Content { get; set; } = string.Empty;
        
        [MaxLength(500)]
        [Display(Name = "Excerpt")]
        public string? Excerpt { get; set; }
        
        [Display(Name = "Category")]
        [MaxLength(50)]
        public string Category { get; set; } = "General";
        
        [Display(Name = "Tags (comma-separated)")]
        public string? Tags { get; set; }
        
        [Display(Name = "Status")]
        public string Status { get; set; } = "Draft";
        
        [Display(Name = "Featured Post")]
        public bool IsFeatured { get; set; } = false;
        
        [Display(Name = "Allow Comments")]
        public bool AllowComments { get; set; } = true;
    }
}

