using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Models.Forum;

namespace UnganaConnect.Controllers
{
    public class ForumMvcController : Controller
    {
        private readonly UnganaConnectDbcontext _context;

        public ForumMvcController(UnganaConnectDbcontext context)
        {
            _context = context;
        }

        // GET: Forum/Home
        public async Task<IActionResult> Home()
        {
            var threads = await _context.ForumThreads
                .Include(t => t.User)
                .Include(t => t.Replies)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            // Fix for missing UserId column in DB: use CreatedById instead of UserId in query
            // So we need to adjust the query to join on CreatedById instead of UserId
            return View(threads);
        }

        // GET: Forum/Thread/5
        public async Task<IActionResult> Thread(int id)
        {
            var thread = await _context.ForumThreads
                .Include(t => t.User)
                .Include(t => t.Replies)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (thread == null)
            {
                return NotFound();
            }

            return View(thread);
        }

        // GET: Forum/CreateThread
        [Authorize]
        public IActionResult CreateThread()
        {
            return View();
        }

        // POST: Forum/CreateThread
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CreateThread([Bind("Title,Content")] ForumThread thread)
        {
            if (ModelState.IsValid)
            {
                thread.CreatedById = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                thread.CreatedAt = DateTime.Now;
                _context.Add(thread);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Home));
            }
            return View(thread);
        }

        // GET: Forum/ModeratorDashboard
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> ModeratorDashboard()
        {
            var threads = await _context.ForumThreads
                .Include(t => t.User)
                .Include(t => t.Replies)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            return View(threads);
        }

        // POST: Forum/PostReply
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> PostReply(int id, string Content)
        {
            if (string.IsNullOrWhiteSpace(Content))
            {
                ModelState.AddModelError("", "Content is required.");
                return RedirectToAction("Thread", new { id });
            }

            var reply = new ForumReply
            {
                ThreadId = id,
                UserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0"),
                Content = Content,
                RepliedAt = DateTime.Now
            };

            _context.ForumReplies.Add(reply);
            await _context.SaveChangesAsync();

            return RedirectToAction("Thread", new { id });
        }

        // POST: Forum/DeleteThread/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> DeleteThread(int id)
        {
            var thread = await _context.ForumThreads.FindAsync(id);
            if (thread != null)
            {
                _context.ForumThreads.Remove(thread);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ModeratorDashboard));
        }
    }
}
