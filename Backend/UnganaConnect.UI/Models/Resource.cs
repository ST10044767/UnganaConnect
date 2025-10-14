namespace UnganaConnect.UI.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string FileUrl { get; set; } = "";
        public string Category { get; set; } = "";
        public DateTime UploadedAt { get; set; }
        public string FileName { get; set; } = "";
        public int CourseId { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; } = "";
        public string FilePath { get; set; } = "";
        public Course? Course { get; set; }
    }

    public class ResourceEngagement
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public int UserId { get; set; }
        public DateTime DownloadedAt { get; set; }
        public Resource? Resource { get; set; }
    }
}
