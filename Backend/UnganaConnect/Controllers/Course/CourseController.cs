using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Models.Training___Learning;
using CourseModel = UnganaConnect.Models.Training___Learning.Course;

namespace UnganaConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly UnganaConnectDbcontext _context;

        public CourseController(UnganaConnectDbcontext context) => _context = context;

        // GET all courses
        [HttpGet("get_course")]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _context.Courses.Include(c => c.Modules).ToListAsync();
            return Ok(courses);
        }

        // CREATE a new course
        [HttpPost("create_course")]
        public async Task<IActionResult> CreateCourse(CourseModel course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCourses), new { id = course.Id }, course);
        }

        // EDIT/UPDATE a course
        [HttpPut("edit_course/{id}")]
        public async Task<IActionResult> EditCourse(int id, CourseModel updatedCourse)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound(new { message = "Course not found." });

            // Update the course fields
            course.Title = updatedCourse.Title;
            course.Category = updatedCourse.Category;

            course.Description = updatedCourse.Description;
            course.Modules = updatedCourse.Modules;

            _context.Courses.Update(course);
            await _context.SaveChangesAsync();

            return Ok(course);
        }

        // DELETE a course
        [HttpDelete("delete_course/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound(new { message = "Course not found." });

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
