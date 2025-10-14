using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Models.Training___Learning;

namespace UnganaConnect.Controllers.Course
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressController : ControllerBase
    {
        private readonly UnganaConnectDbcontext _context;

        public ProgressController(UnganaConnectDbcontext context)
        {
            _context = context;
        }

        // GET user's progress for a course
        [Authorize]
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetCourseProgress(int courseId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var course = await _context.Courses
                .Include(c => c.Modules)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                return NotFound("Course not found.");

            var progress = await _context.UserProgress
                .Where(p => p.UserId == userId && course.Modules.Any(m => m.Id == p.ModuleId))
                .ToListAsync();

            var moduleProgress = course.Modules.Select(m => new
            {
                ModuleId = m.Id,
                ModuleTitle = m.Title,
                IsCompleted = progress.Any(p => p.ModuleId == m.Id && p.IsCompleted)
            }).ToList();

            var completedModules = moduleProgress.Count(mp => mp.IsCompleted);
            var totalModules = moduleProgress.Count;
            var completionPercentage = totalModules > 0 ? (completedModules * 100) / totalModules : 0;

            return Ok(new
            {
                CourseId = courseId,
                CourseTitle = course.Title,
                CompletedModules = completedModules,
                TotalModules = totalModules,
                CompletionPercentage = completionPercentage,
                ModuleProgress = moduleProgress
            });
        }

        // GET user's overall progress
        [Authorize]
        [HttpGet("overview")]
        public async Task<IActionResult> GetUserProgressOverview()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .ThenInclude(c => c.Modules)
                .Where(e => e.UserId == userId)
                .ToListAsync();

            var progress = await _context.UserProgress
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var courseProgress = enrollments.Select(enrollment => new
            {
                CourseId = enrollment.CourseId,
                CourseTitle = enrollment.Course.Title,
                TotalModules = enrollment.Course.Modules.Count,
                CompletedModules = progress.Count(p => enrollment.Course.Modules.Any(m => m.Id == p.ModuleId && p.IsCompleted)),
                IsCourseCompleted = enrollment.IsCompleted
            }).ToList();

            return Ok(courseProgress);
        }

        // MARK module as completed
        [Authorize]
        [HttpPost("complete/{moduleId}")]
        public async Task<IActionResult> MarkModuleCompleted(int moduleId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var module = await _context.Modules.FindAsync(moduleId);
            if (module == null)
                return NotFound("Module not found.");

            var existingProgress = await _context.UserProgress
                .FirstOrDefaultAsync(p => p.UserId == userId && p.ModuleId == moduleId);

            if (existingProgress != null)
            {
                existingProgress.IsCompleted = true;
            }
            else
            {
                _context.UserProgress.Add(new UserProgress
                {
                    UserId = userId,
                    ModuleId = moduleId,
                    IsCompleted = true
                });
            }

            await _context.SaveChangesAsync();

            // Check if course is completed
            await CheckAndUpdateCourseCompletion(userId, module.CourseId);

            return Ok(new { message = "Module marked as completed." });
        }

        // UNMARK module as completed
        [Authorize]
        [HttpPost("uncomplete/{moduleId}")]
        public async Task<IActionResult> MarkModuleIncomplete(int moduleId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var progress = await _context.UserProgress
                .FirstOrDefaultAsync(p => p.UserId == userId && p.ModuleId == moduleId);

            if (progress == null)
                return NotFound("Progress record not found.");

            progress.IsCompleted = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Module marked as incomplete." });
        }

        private async Task CheckAndUpdateCourseCompletion(string userId, int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Modules)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null) return;

            var completedModules = await _context.UserProgress
                .CountAsync(p => p.UserId == userId && 
                               course.Modules.Any(m => m.Id == p.ModuleId) && 
                               p.IsCompleted);

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (enrollment != null && completedModules == course.Modules.Count)
            {
                enrollment.IsCompleted = true;
                enrollment.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}