using Elfie.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using UnganaConnect.Data;
using UnganaConnect.Models.Resources_Repo;
using UnganaConnect.Models.Training___Learning;
using UnganaConnect.Service;

namespace UnganaConnect.Controllers.Resources
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourceController : ControllerBase
    {
        private readonly FileServices _fileServices;
        private readonly UnganaConnectDbcontext _context;

        public ResourceController(FileServices fileServices, UnganaConnectDbcontext context)
        {
            _fileServices = fileServices;
            _context = context;
        }

        // 🔹 Create a folder for a course (admin)
        [HttpPost("create-folder/{courseName}")]
        public async Task<IActionResult> CreateFolder(string courseName)
        {
            var path = await _fileServices.CreateCourseFolderAsync(courseName);
            return Ok(new { message = "Course folder created successfully.", path });
        }

        // 🔹 Upload a file to a specific course (admin)
        [HttpPost("upload/{courseId}")]
        public async Task<IActionResult> Upload(int courseId, IFormFile file)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return NotFound($"Course with ID {courseId} not found.");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var path = await _fileServices.UploadResourceAsync(course.Title, file);

            var resource = new CResource
            {
                FileName = file.FileName,
                FilePath = path,
                FileType = file.ContentType,
                FileSize = file.Length,
                CourseId = course.Id,
                UploadedAt = DateTime.UtcNow
            };

            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();

            return Ok(new { message = "File uploaded successfully.", resource });
        }

        // 🔹 List all resources for a course
        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetCourseResources(int courseId)
        {
            var course = await _context.Courses.Include(c => c.Modules)
                                               .FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null)
                return NotFound($"Course with ID {courseId} not found.");

            var resources = await _context.Resources
                                          .Where(r => r.CourseId == courseId)
                                          .ToListAsync();

            return Ok(new { course = course.Title, resources });
        }

        // 🔹 Download a file
        [HttpGet("{courseId}/download/{fileName}")]
        public async Task<IActionResult> DownloadFile(int courseId, string fileName)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return NotFound($"Course with ID {courseId} not found.");

            var fileBytes = await _fileServices.GetResourceFileAsync(course.Title, fileName);
            if (fileBytes == null || fileBytes.Length == 0)
                return NotFound("File not found.");

            return File(fileBytes, "application/octet-stream", fileName);
        }

        // 🔹 Delete a resource
        [HttpDelete("{courseId}/delete/{fileName}")]
        public async Task<IActionResult> DeleteFile(int courseId, string fileName)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return NotFound($"Course with ID {courseId} not found.");

            _fileServices.DeleteResource(course.Title, fileName);

            var resource = await _context.Resources
                                         .FirstOrDefaultAsync(r => r.FileName == fileName && r.CourseId == courseId);
            if (resource != null)
            {
                _context.Resources.Remove(resource);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "File deleted successfully." });
        }
    }
}
