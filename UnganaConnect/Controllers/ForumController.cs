using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Frontend.Models;

namespace UnganaConnect.Frontend.Controllers
{
    public class ForumController : Controller
    {
        private readonly UnganaConnectDbContext _context;

        public ForumController(UnganaConnectDbContext context)
        {
            _context = context;
        }

        // GET: /Forum
        public async Task<IActionResult> Index()
        {
            var categories = await _context.ForumCategories
                .Include(c => c.Topics)
                .ToListAsync();

            var topics = await _context.ForumTopics
                .Include(t => t.Category)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var viewModel = new ForumViewModel
            {
                Categories = categories.Select(c => new ForumCategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Color = c.Color,
                    PostsCount = c.Topics.Count
                }).ToList(),

                RecentTopics = topics.Select(t => new ForumTopicViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    CategoryName = t.Category?.Name ?? "Uncategorized",
                    Author = t.Author,
                    AuthorRole = t.AuthorRole,
                    AuthorOrg = t.AuthorOrg,
                    CreatedAt = t.CreatedAt,
                    Replies = t.Replies,
                    Views = t.Views,
                    Tags = string.IsNullOrEmpty(t.Tags)
                        ? new List<string>()
                        : t.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(tag => tag.Trim()).ToList(),
                    Excerpt = t.Content.Length > 100 ? t.Content[..100] + "..." : t.Content
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: /Forum/CreateTopic
        [HttpGet]
        public async Task<IActionResult> CreateTopic()
        {
            ViewBag.Categories = await _context.ForumCategories.ToListAsync();
            return View();
        }

        // POST: /Forum/CreateTopic
        [HttpPost]
        public async Task<IActionResult> CreateTopic(CreateTopicViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.ForumCategories.ToListAsync();
                return View(model);
            }

            var topic = new ForumTopic
            {
                Title = model.Title,
                Content = model.Content,
                Author = HttpContext.Session.GetString("UserEmail") ?? "Anonymous",
                AuthorRole = HttpContext.Session.GetString("Role") ?? "Student",
                AuthorOrg = "Ungana Connect",
                Tags = model.Tags,
                CategoryId = model.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ForumTopics.Add(topic);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Topic created successfully!";
            return RedirectToAction("Index");
        }

        // GET: /Forum/Topic/5
        [HttpGet]
        public async Task<IActionResult> Topic(int id)
        {
            var topic = await _context.ForumTopics
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (topic == null) return NotFound();

            topic.Views++;
            await _context.SaveChangesAsync();

            var replies = await _context.ForumReplies
                .Where(r => r.TopicId == id)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();

            var viewModel = new ForumTopicDetailViewModel
            {
                Id = topic.Id,
                Title = topic.Title,
                CategoryName = topic.Category?.Name ?? "Uncategorized",
                Author = topic.Author,
                AuthorRole = topic.AuthorRole,
                AuthorOrg = topic.AuthorOrg,
                CreatedAt = topic.CreatedAt,
                Replies = topic.Replies,
                Views = topic.Views,
                IsPinned = topic.IsPinned,
                IsAnswered = topic.IsAnswered,
                Tags = string.IsNullOrEmpty(topic.Tags)
                    ? new List<string>()
                    : topic.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(tag => tag.Trim()).ToList(),
                Excerpt = topic.Content,
                RepliesList = replies
            };

            return View(viewModel);
        }

        // POST: /Forum/Reply
        [HttpPost]
        public async Task<IActionResult> Reply(int topicId, string content)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Error"] = "Please log in to reply to topics.";
                return RedirectToAction("Topic", new { id = topicId });
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Reply content cannot be empty.";
                return RedirectToAction("Topic", new { id = topicId });
            }

            var reply = new ForumReply
            {
                TopicId = topicId,
                Content = content,
                Author = userEmail,
                CreatedAt = DateTime.UtcNow
            };

            _context.ForumReplies.Add(reply);

            var topic = await _context.ForumTopics.FindAsync(topicId);
            if (topic != null) topic.Replies++;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Reply posted successfully!";
            return RedirectToAction("Topic", new { id = topicId });
        }
    }
}