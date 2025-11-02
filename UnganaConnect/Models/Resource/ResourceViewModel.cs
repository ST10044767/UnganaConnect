using System.ComponentModel.DataAnnotations;
using UnganaConnect.Frontend.Models;
using UnganaConnect.Models.Resource;

namespace UnganaConnect.Frontend.Models
{

    
    public class Resource
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Downloads { get; set; }
        public DateTime UploadDate { get; set; }
        public string Tags { get; set; } = string.Empty; // Comma-separated
        public string Url { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
    public class ResourceLibraryViewModel
    {
        public List<Resource> Resources { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public List<string> Types { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
        public string SelectedCategory { get; set; } = "all";
        public ResourceStatsViewModel Stats { get; set; } = new();

    }

    public class ResourceStatsViewModel
    {
        public int TotalResources { get; set; }
        public int TotalDownloads { get; set; }
        public int ThisMonth { get; set; }
        public int Contributors { get; set; }
    }
}