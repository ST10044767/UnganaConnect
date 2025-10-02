namespace UnganaConnect.Models.Forum
{
    using UnganaConnect.Models.Users;

    // 5. Community Forum
    public class ForumThread
    {
        public int Id { get; set; }
        public int CreatedById { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public ICollection<ForumReply> Replies { get; set; }
    }

}
