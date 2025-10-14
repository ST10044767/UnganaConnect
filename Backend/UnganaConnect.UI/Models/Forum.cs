namespace UnganaConnect.UI.Models
{
    public class Thread
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLocked { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public List<Reply> Replies { get; set; } = new();
        public List<Upvote> Upvotes { get; set; } = new();
    }

    public class Reply
    {
        public int Id { get; set; }
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public int ThreadId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public List<Upvote> Upvotes { get; set; } = new();
    }

    public class Upvote
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ThreadId { get; set; }
        public int? ReplyId { get; set; }
    }
}
