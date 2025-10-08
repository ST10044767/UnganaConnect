using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.Frontend.Models
{
    public class ForumCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Topics { get; set; }
        public int Posts { get; set; }
        public string Color { get; set; } = string.Empty;
    }



    public class ForumTopicViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string AuthorRole { get; set; } = string.Empty;
        public string AuthorOrg { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int Replies { get; set; }
        public int Views { get; set; }
        public string LastActivity { get; set; } = string.Empty;
        public string LastActivityBy { get; set; } = string.Empty;
        public bool IsPinned { get; set; }
        public bool IsAnswered { get; set; }
        public List<string> Tags { get; set; } = new();
        public string Excerpt { get; set; } = string.Empty;
    }

    public class TrendingTopicViewModel
    {
        public string Tag { get; set; } = string.Empty;
        public int Posts { get; set; }
    }

    public class TopContributorViewModel
    {
        public string Name { get; set; } = string.Empty;
        public int Posts { get; set; }
        public int Reputation { get; set; }
    }

    public class ForumIndexViewModel
    {
        public List<ForumCategoryViewModel> Categories { get; set; } = new();
        public List<ForumTopicViewModel> RecentTopics { get; set; } = new();
        public List<TrendingTopicViewModel> TrendingTopics { get; set; } = new();
        public List<TopContributorViewModel> TopContributors { get; set; } = new();
    }
}
