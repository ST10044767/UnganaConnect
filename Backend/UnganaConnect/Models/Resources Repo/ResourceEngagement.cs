namespace UnganaConnect.Models.Resources_Repo
{
    public class ResourceEngagement
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public int UserId { get; set; }
        public DateTime DownloadedAt { get; set; }
    }
}
