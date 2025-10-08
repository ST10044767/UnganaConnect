using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.Frontend.Models
{
    public class ResourceViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Downloads { get; set; }
        public DateTime UploadDate { get; set; }
        public List<string> Tags { get; set; } = new();
        public string Author { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ResourceLibraryViewModel
    {
        public List<ResourceViewModel> Resources { get; set; } = new();
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