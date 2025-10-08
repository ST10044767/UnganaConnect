using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.Frontend.Models
{
    public class ForumCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Topics { get; set; }
        public int Posts { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class ForumTopic
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string AuthorRole { get; set; } = string.Empty;
        public string AuthorOrg { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public int Replies { get; set; }
        public int Views { get; set; }
        public string LastActivity { get; set; } = string.Empty;
        public string LastActivityBy { get; set; } = string.Empty;
        public bool IsPinned { get; set; }
        public bool IsAnswered { get; set; }
        public List<string> Tags { get; set; } = new();
        public string Excerpt { get; set; } = string.Empty;
    }

    public class CreateTopicViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public int CategoryId { get; set; }
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public string Tags { get; set; } = string.Empty;
    }

    public class ForumViewModel
    {
        public List<ForumCategory> Categories { get; set; } = new();
        public List<ForumTopic> RecentTopics { get; set; } = new();
        public List<TrendingTopic> TrendingTopics { get; set; } = new();
        public List<TopContributor> TopContributors { get; set; } = new();
    }

    public class TrendingTopic
    {
        public string Tag { get; set; } = string.Empty;
        public int Posts { get; set; }
    }

    public class TopContributor
    {
        public string Name { get; set; } = string.Empty;
        public int Posts { get; set; }
        public int Reputation { get; set; }
    }
}