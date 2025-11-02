using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnganaConnect.Frontend.Models
{
    // ==========================
    // Core EF Models
    // ==========================

    public class ForumCategory
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        // Navigation property: a category can have many topics
        public List<ForumTopic> Topics { get; set; } = new();
    }

    public class ForumTopic
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;
        public string AuthorRole { get; set; } = string.Empty;
        public string AuthorOrg { get; set; } = "Ungana Connect";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Replies { get; set; }
        public int Views { get; set; }

        public bool IsPinned { get; set; }
        public bool IsAnswered { get; set; }

        public string Tags { get; set; } = string.Empty;

        // Foreign key
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public ForumCategory Category { get; set; } = null!;

        // Short excerpt for listing
        public string Excerpt { get; set; } = string.Empty;

        // Navigation property for replies
        public List<ForumReply> RepliesList { get; set; } = new();
    }

    public class ForumReply
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(Topic))]
        public int TopicId { get; set; }
        public ForumTopic Topic { get; set; } = null!;
    }

    // ==========================
    // ViewModels
    // ==========================

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

    public class ForumCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int PostsCount { get; set; }
    }

    public class ForumTopicViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string AuthorRole { get; set; } = string.Empty;
        public string AuthorOrg { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int Replies { get; set; }
        public int Views { get; set; }
        public bool IsPinned { get; set; }
        public bool IsAnswered { get; set; }
        public List<string> Tags { get; set; } = new();
        public string Excerpt { get; set; } = string.Empty;
    }

    public class ForumTopicDetailViewModel : ForumTopicViewModel
    {
        public List<ForumReply> RepliesList { get; set; } = new();
    }

    public class ForumViewModel
    {
        public List<ForumCategoryViewModel> Categories { get; set; } = new();
        public List<ForumTopicViewModel> RecentTopics { get; set; } = new();
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
