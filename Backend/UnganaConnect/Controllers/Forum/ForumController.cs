using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UnganaConnect.Models;
using UnganaConnect.Data;
using static UnganaConnect.Models.Forum.CommunityForum;

namespace UnganaConnect.Controllers.Forum
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForumController : ControllerBase
    {
        private readonly UnganaConnectDbcontext _context;

        public ForumController(UnganaConnectDbcontext context)
        {
            _context = context;
        }

        // -----------------------------------------------------------
        // 🔹 THREADS
        // -----------------------------------------------------------

        // GET: api/forum/threads
        [HttpGet("threads")]
        public async Task<IActionResult> GetThreads()
        {
            var threads = await _context.Threads
                .Include(t => t.Users)
                .Include(t => t.Upvotes)
                .Where(t => t.IsApproved)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return Ok(threads);
        }

        // GET: api/forum/threads/{id}
        [HttpGet("threads/{id}")]
        public async Task<IActionResult> GetThread(int id)
        {
            var thread = await _context.Threads
                .Include(t => t.Replies)
                    .ThenInclude(r => r.User)
                .Include(t => t.Upvotes)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsApproved);

            if (thread == null) return NotFound("Thread not found or not approved.");
            return Ok(thread);
        }

        // POST: api/forum/threads
        [Authorize]
        [HttpPost("threads")]
        public async Task<IActionResult> CreateThread([FromBody] Models.Forum.CommunityForum.Thread thread)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            thread.UserId = userId;
            thread.CreatedAt = DateTime.UtcNow;
            thread.IsApproved = false; // Requires admin approval

            _context.Threads.Add(thread);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetThread), new { id = thread.Id }, thread);
        }

        // PUT: api/forum/threads/{id}
        [Authorize]
        [HttpPut("threads/{id}")]
        public async Task<IActionResult> UpdateThread(int id, [FromBody] Models.Forum.CommunityForum.Thread updated)
        {
            var thread = await _context.Threads.FindAsync(id);
            if (thread == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var isAdmin = User.IsInRole("Admin");

            if (thread.UserId != userId && !isAdmin)
                return Forbid();

            thread.Title = updated.Title;
            thread.Content = updated.Content;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/forum/threads/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("threads/{id}")]
        public async Task<IActionResult> DeleteThread(int id)
        {
            var thread = await _context.Threads.FindAsync(id);
            if (thread == null) return NotFound();

            _context.Threads.Remove(thread);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // -----------------------------------------------------------
        // 🔹 REPLIES
        // -----------------------------------------------------------

        // POST: api/forum/threads/{threadId}/reply
        [Authorize]
        [HttpPost("threads/{threadId}/reply")]
        public async Task<IActionResult> AddReply(int threadId, [FromBody] Reply reply)
        {
            var thread = await _context.Threads.FindAsync(threadId);
            if (thread == null || !thread.IsApproved || thread.IsLocked)
                return BadRequest("Thread is unavailable.");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            reply.ThreadId = threadId;
            reply.UserId = userId;
            reply.CreatedAt = DateTime.UtcNow;

            _context.Replies.Add(reply);
            await _context.SaveChangesAsync();

            return Ok(reply);
        }

        // PUT: api/forum/replies/{id}
        [Authorize]
        [HttpPut("replies/{id}")]
        public async Task<IActionResult> EditReply(int id, [FromBody] Reply updatedReply)
        {
            var reply = await _context.Replies.FindAsync(id);
            if (reply == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var isAdmin = User.IsInRole("Admin");

            if (reply.UserId != userId && !isAdmin)
                return Forbid();

            reply.Content = updatedReply.Content;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/forum/replies/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("replies/{id}")]
        public async Task<IActionResult> DeleteReply(int id)
        {
            var reply = await _context.Replies.FindAsync(id);
            if (reply == null) return NotFound();

            _context.Replies.Remove(reply);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // -----------------------------------------------------------
        // 🔹 UPVOTES
        // -----------------------------------------------------------

        // POST: api/forum/threads/{id}/upvote
        [Authorize]
        [HttpPost("threads/{id}/upvote")]
        public async Task<IActionResult> UpvoteThread(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var existing = await _context.Upvotes
                .FirstOrDefaultAsync(u => u.ThreadId == id && u.UserId == userId);

            if (existing != null)
                _context.Upvotes.Remove(existing);
            else
                _context.Upvotes.Add(new Upvote { ThreadId = id, UserId = userId });

            await _context.SaveChangesAsync();
            return Ok();
        }

        // POST: api/forum/replies/{id}/upvote
        [Authorize]
        [HttpPost("replies/{id}/upvote")]
        public async Task<IActionResult> UpvoteReply(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var existing = await _context.Upvotes
                .FirstOrDefaultAsync(u => u.ReplyId == id && u.UserId == userId);

            if (existing != null)
                _context.Upvotes.Remove(existing);
            else
                _context.Upvotes.Add(new Upvote { ReplyId = id, UserId = userId });

            await _context.SaveChangesAsync();
            return Ok();
        }

        // -----------------------------------------------------------
        // 🔹 ADMIN MODERATION
        // -----------------------------------------------------------

        // GET: api/forum/admin/pending
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/pending")]
        public async Task<IActionResult> GetPendingThreads()
        {
            var pending = await _context.Threads
                .Where(t => !t.IsApproved)
                .ToListAsync();

            return Ok(pending);
        }

        // PUT: api/forum/admin/approve/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("admin/approve/{id}")]
        public async Task<IActionResult> ApproveThread(int id)
        {
            var thread = await _context.Threads.FindAsync(id);
            if (thread == null) return NotFound();

            thread.IsApproved = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/forum/admin/lock/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("admin/lock/{id}")]
        public async Task<IActionResult> LockThread(int id)
        {
            var thread = await _context.Threads.FindAsync(id);
            if (thread == null) return NotFound();

            thread.IsLocked = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
