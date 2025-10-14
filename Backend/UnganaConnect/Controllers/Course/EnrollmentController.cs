using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Models.Training___Learning;

namespace UnganaConnect.Controllers.Course
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly UnganaConnectDbcontext _context;

        public EnrollmentController(UnganaConnectDbcontext context)
        {
            _context = context;
        }

        
        // Enroll in a course
        [Authorize]
        [HttpPost("{courseId}/enroll")]
        public async Task<IActionResult> EnrollUser(int courseId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            // check if already enrolled
            var exists = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);

            if (exists != null)
                return BadRequest("User already enrolled in this course.");

            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId,
                EnrolledAt = DateTime.UtcNow
            };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(enrollment);
        }

        // Get user's enrolled courses
        [Authorize]
        [HttpGet("my-enrollments")]
        public async Task<IActionResult> GetUserEnrollments()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var enrollments = await _context.Enrollments
                .Where(e => e.UserId == userId)
                .Include(e => e.Course)
                .ToListAsync();

            return Ok(enrollments);
        }

        // Drop/unenroll from course
        [Authorize]
        [HttpDelete("{courseId}/unenroll")]
        public async Task<IActionResult> UnenrollUser(int courseId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);

            if (enrollment == null)
                return NotFound("Enrollment not found.");

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return Ok("User unenrolled successfully.");
        }
        }
    }
