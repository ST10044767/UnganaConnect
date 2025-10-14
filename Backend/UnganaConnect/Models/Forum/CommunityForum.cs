using System.ComponentModel.DataAnnotations;
using UnganaConnect.Models.Users;

namespace UnganaConnect.Models.Forum
{
    public class CommunityForum
    {
        public class Thread
        {
            public int Id { get; set; }
            
            [Required]
            [StringLength(200)]
            public string Title { get; set; } = string.Empty;
            
            [Required]
            [StringLength(2000)]
            public string Content { get; set; } = string.Empty;
            
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            
            public bool IsApproved { get; set; } = false; // Default to false for moderation
            public bool IsLocked { get; set; } = false;

            [Required]
            public int UserId { get; set; }

            public User? Users { get; set; }
            public ICollection<Reply> Replies { get; set; } = new List<Reply>();
            public ICollection<Upvote> Upvotes { get; set; } = new List<Upvote>();
        }

        public class Reply
        {
            public int Id { get; set; }
            
            [Required]
            [StringLength(1000)]
            public string Content { get; set; } = string.Empty;
            
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            [Required]
            public int ThreadId { get; set; }
            public Thread? Thread { get; set; }

            [Required]
            public int UserId { get; set; }
            public User? User { get; set; }
            public ICollection<Upvote> Upvotes { get; set; } = new List<Upvote>();
        }

        public class Upvote
        {
            public int Id { get; set; }
            
            [Required]
            public int UserId { get; set; }
            
            public int? ThreadId { get; set; }
            public int? ReplyId { get; set; }
            
            // Navigation properties
            public User? User { get; set; }
            public Thread? Thread { get; set; }
            public Reply? Reply { get; set; }
        }
    }
}
