using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Models.Training___Learning;
using UnganaConnect.Models.Event_Management;
using UnganaConnect.Models.Forum;
using UnganaConnect.Models.Resources_Repo;

namespace UnganaConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UnganaConnectDbcontext _context;

        public AdminController(UnganaConnectDbcontext context)
        {
            _context = context;
        }

        // GET dashboard statistics
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalCourses = await _context.Courses.CountAsync();
            var totalEnrollments = await _context.Enrollments.CountAsync();
            var totalEvents = await _context.Events.CountAsync();
            var totalThreads = await _context.Threads.CountAsync();
            var totalResources = await _context.Resources.CountAsync();

            var recentEnrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.User)
                .OrderByDescending(e => e.EnrolledAt)
                .Take(5)
                .ToListAsync();

            var recentEvents = await _context.Events
                .OrderByDescending(e => e.StartDate)
                .Take(5)
                .ToListAsync();

            return Ok(new
            {
                Statistics = new
                {
                    TotalUsers = totalUsers,
                    TotalCourses = totalCourses,
                    TotalEnrollments = totalEnrollments,
                    TotalEvents = totalEvents,
                    TotalThreads = totalThreads,
                    TotalResources = totalResources
                },
                RecentActivity = new
                {
                    RecentEnrollments = recentEnrollments,
                    RecentEvents = recentEvents
                }
            });
        }

        // GET all users with pagination
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var users = await _context.Users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalUsers = await _context.Users.CountAsync();

            return Ok(new
            {
                Users = users,
                TotalCount = totalUsers,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize)
            });
        }

        // UPDATE user role
        [HttpPut("users/{userId}/role")]
        public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] UpdateRoleRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            user.Role = request.Role;
            await _context.SaveChangesAsync();

            return Ok(new { message = "User role updated successfully." });
        }

        // GET course analytics
        [HttpGet("analytics/courses")]
        public async Task<IActionResult> GetCourseAnalytics()
        {
            var courseStats = await _context.Courses
                .Include(c => c.Modules)
                .Include(c => c.Enrollments)
                .Select(c => new
                {
                    CourseId = c.Id,
                    CourseTitle = c.Title,
                    ModuleCount = c.Modules.Count,
                     EnrollmentCount = c.Enrollments.Count,
                     CompletionCount = c.Enrollments.Count(e => e.IsCompleted)
                })
                .ToListAsync();

            return Ok(courseStats);
        }

        // GET event analytics
        [HttpGet("analytics/events")]
        public async Task<IActionResult> GetEventAnalytics()
        {
             var eventStats = await _context.Events
                 .Include(e => e.EventRegistrations)
                 .Select(e => new
                 {
                     EventId = e.Id,
                     EventTitle = e.Title,
                     StartDate = e.StartDate,
                     RegistrationCount = e.EventRegistrations.Count
                 })
                .ToListAsync();

            return Ok(eventStats);
        }

        // GET forum analytics
        [HttpGet("analytics/forum")]
        public async Task<IActionResult> GetForumAnalytics()
        {
            var forumStats = await _context.Threads
                .Include(t => t.Replies)
                .Include(t => t.Upvotes)
                .Select(t => new
                {
                    ThreadId = t.Id,
                    ThreadTitle = t.Title,
                    ReplyCount = t.Replies.Count,
                    UpvoteCount = t.Upvotes.Count,
                    IsApproved = t.IsApproved,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return Ok(forumStats);
        }

        // GET pending forum threads
        [HttpGet("forum/pending")]
        public async Task<IActionResult> GetPendingThreads()
        {
            var pendingThreads = await _context.Threads
                .Include(t => t.Users)
                .Where(t => !t.IsApproved)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();

            return Ok(pendingThreads);
        }

        // APPROVE forum thread
        [HttpPut("forum/approve/{threadId}")]
        public async Task<IActionResult> ApproveThread(int threadId)
        {
            var thread = await _context.Threads.FindAsync(threadId);
            if (thread == null)
                return NotFound("Thread not found.");

            thread.IsApproved = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thread approved successfully." });
        }

        // DELETE user
        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully." });
        }
    }

    public class UpdateRoleRequest
    {
        public string Role { get; set; } = string.Empty;
    }
}
