namespace UnganaConnect.Models.Forum
{
    using UnganaConnect.Models.Users;

    public class ForumReply
    {
        public int Id { get; set; }
        public int ThreadId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTime RepliedAt { get; set; }
        public int Upvotes { get; set; }

        // Navigation properties
        public User User { get; set; }
        public ForumThread Thread { get; set; }
    }
}
