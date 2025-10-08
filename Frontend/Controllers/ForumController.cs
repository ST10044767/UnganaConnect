using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Frontend.Models;

namespace UnganaConnect.Frontend.Controllers
{
    public class ForumController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new ForumIndexViewModel
            {
                Categories = GetForumCategories().Select(c => new ForumCategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Topics = c.Topics,
                    Posts = c.Posts,
                    Color = c.Color
                }).ToList(),
                RecentTopics = GetRecentTopics().Select(t => new ForumTopicViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Category = t.Category,
                    Author = t.Author,
                    AuthorRole = t.AuthorRole,
                    AuthorOrg = t.AuthorOrg,
                    CreatedAt = DateTime.Now, // or parse if needed
                    Replies = t.Replies,
                    Views = t.Views,
                    LastActivity = t.LastActivity,
                    LastActivityBy = t.LastActivityBy,
                    IsPinned = t.IsPinned,
                    IsAnswered = t.IsAnswered,
                    Tags = t.Tags,
                    Excerpt = t.Excerpt
                }).ToList(),
                TrendingTopics = GetTrendingTopics().Select(t => new TrendingTopicViewModel
                {
                    Tag = t.Tag,
                    Posts = t.Posts
                }).ToList(),
                TopContributors = GetTopContributors().Select(c => new TopContributorViewModel
                {
                    Name = c.Name,
                    Posts = c.Posts,
                    Reputation = c.Reputation
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult CreateTopic()
        {
            ViewBag.Categories = GetForumCategories();
            return View();
        }

        [HttpPost]
        public IActionResult CreateTopic(CreateTopicViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Save topic to database
                TempData["Success"] = "Topic created successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.Categories = GetForumCategories();
            return View(model);
        }

        private List<ForumCategory> GetForumCategories()
        {
            return new List<ForumCategory>
            {
                new() { Id = 1, Name = "General Discussion", Description = "General topics about CSO operations and experiences", Topics = 45, Posts = 234, Color = "bg-blue-100 text-blue-800" },
                new() { Id = 2, Name = "Grant Writing & Fundraising", Description = "Tips, strategies, and experiences in securing funding", Topics = 78, Posts = 312, Color = "bg-green-100 text-green-800" },
                new() { Id = 3, Name = "Project Management", Description = "Best practices for managing CSO projects and programs", Topics = 34, Posts = 156, Color = "bg-purple-100 text-purple-800" },
                new() { Id = 4, Name = "Partnerships & Networking", Description = "Building relationships and collaborative opportunities", Topics = 23, Posts = 89, Color = "bg-orange-100 text-orange-800" },
                new() { Id = 5, Name = "Technology & Digital Tools", Description = "Digital solutions and technology adoption for CSOs", Topics = 29, Posts = 145, Color = "bg-cyan-100 text-cyan-800" },
                new() { Id = 6, Name = "Impact Measurement", Description = "Measuring and reporting on program outcomes and impact", Topics = 19, Posts = 67, Color = "bg-yellow-100 text-yellow-800" }
            };
        }

        private List<ForumTopic> GetRecentTopics()
        {
            return new List<ForumTopic>
            {
                new() { Id = 1, Title = "Best practices for donor stewardship in post-pandemic era", Category = "Grant Writing & Fundraising", Author = "Amina Hassan", AuthorRole = "Program Director", AuthorOrg = "Hope Foundation Kenya", CreatedAt = "2 hours ago", Replies = 12, Views = 45, LastActivity = "30 minutes ago", LastActivityBy = "David Kiptoo", IsPinned = true, IsAnswered = false, Tags = new() { "donor relations", "fundraising", "best practices" }, Excerpt = "Looking for innovative approaches to maintain strong donor relationships in the current funding landscape..." },
                new() { Id = 2, Title = "Seeking collaboration for youth empowerment program in West Africa", Category = "Partnerships & Networking", Author = "Kofi Asante", AuthorRole = "Executive Director", AuthorOrg = "Youth Action Ghana", CreatedAt = "5 hours ago", Replies = 8, Views = 32, LastActivity = "1 hour ago", LastActivityBy = "Sarah Mensah", IsPinned = false, IsAnswered = true, Tags = new() { "partnership", "youth", "west africa", "collaboration" }, Excerpt = "We are implementing a youth empowerment program across Ghana and looking for partners in neighboring countries..." },
                new() { Id = 3, Title = "Digital transformation: Which project management tools work best?", Category = "Technology & Digital Tools", Author = "Rachel Mwangi", AuthorRole = "Operations Manager", AuthorOrg = "Education First Uganda", CreatedAt = "1 day ago", Replies = 15, Views = 67, LastActivity = "2 hours ago", LastActivityBy = "Ahmed Ali", IsPinned = false, IsAnswered = true, Tags = new() { "project management", "digital tools", "software" }, Excerpt = "Our organization is transitioning to digital project management. What tools have worked well for your CSO?" },
                new() { Id = 4, Title = "Impact measurement frameworks for education programs", Category = "Impact Measurement", Author = "Dr. Fatima Ndour", AuthorRole = "Research Director", AuthorOrg = "Education Excellence Senegal", CreatedAt = "2 days ago", Replies = 7, Views = 28, LastActivity = "4 hours ago", LastActivityBy = "Moses Kiprotich", IsPinned = false, IsAnswered = false, Tags = new() { "impact measurement", "education", "evaluation" }, Excerpt = "Looking for recommendations on comprehensive impact measurement frameworks specifically designed for education interventions..." }
            };
        }

        private List<TrendingTopic> GetTrendingTopics()
        {
            return new List<TrendingTopic>
            {
                new() { Tag = "digital transformation", Posts = 23 },
                new() { Tag = "grant writing", Posts = 19 },
                new() { Tag = "partnership", Posts = 15 },
                new() { Tag = "impact measurement", Posts = 12 },
                new() { Tag = "fundraising", Posts = 11 }
            };
        }

        private List<TopContributor> GetTopContributors()
        {
            return new List<TopContributor>
            {
                new() { Name = "Sarah Mensah", Posts = 45, Reputation = 892 },
                new() { Name = "David Kiptoo", Posts = 38, Reputation = 756 },
                new() { Name = "Dr. Amara Okafor", Posts = 31, Reputation = 623 },
                new() { Name = "Ahmed Ali", Posts = 28, Reputation = 534 },
                new() { Name = "Rachel Mwangi", Posts = 24, Reputation = 487 }
            };
        }
    }
}