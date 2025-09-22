using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Models.Training___Learning;

namespace UnganaConnect.Controllers.Course
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : Controller
    {
        private readonly UnganaConnectDbcontext _context;

        public EnrollmentController(UnganaConnectDbcontext context)
        {
            _context = context;
        }

        
            // Enroll in a course
            [HttpPost("{courseId}/enroll/{userId}")]
            public async Task<IActionResult> EnrollUser(int courseId, string userId)
            {
                // check if already enrolled
                var exists = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);

                if (exists != null)
                    return BadRequest("User already enrolled in this course.");

                var enrollment = new Enrollment
                {
                    UserId = userId,
                    CourseId = courseId
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                return Ok(enrollment);
            }

            // Get user’s enrolled courses
            [HttpGet("user/{userId}")]
            public async Task<IActionResult> GetUserEnrollments(string userId)
            {
                var enrollments = await _context.Enrollments
                    .Where(e => e.UserId == userId)
                    .Include(e => e.Course)
                    .ToListAsync();

                return Ok(enrollments);
            }

            // Drop/unenroll from course
            [HttpDelete("{courseId}/unenroll/{userId}")]
            public async Task<IActionResult> UnenrollUser(int courseId, string userId)
            {
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
