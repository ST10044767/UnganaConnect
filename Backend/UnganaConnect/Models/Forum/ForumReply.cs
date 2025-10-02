namespace UnganaConnect.Models.Forum
{

    public class ForumReply
    {
        public int Id { get; set; }
        public int ThreadId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTime RepliedAt { get; set; }
        public int Upvotes { get; set; }
    }
}
