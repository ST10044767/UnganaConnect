using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using UnganaConnect.Data;
using UnganaConnect.Models;
using UnganaConnect.Models.Training___Learning;
using UnganaConnect.Service;



namespace UnganaConnect.Controllers.Course
{

    [ApiController]
    [Route("api/[controller]")]
    public class ModuleController : ControllerBase
    {
        private readonly UnganaConnectDbcontext _context;
        private readonly IWebHostEnvironment _env;
        private readonly BlobServices _blobServices;

        public ModuleController(UnganaConnectDbcontext context, IWebHostEnvironment env, BlobServices blobServices)
        {
            _context = context;//From Dbconext
            _env = env;
            _blobServices = blobServices;
        }

   
        // GET: Get all modules for a course
        
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetModulesByCourse(int courseId)
        {
            var modules = await _context.Modules
                .Where(m => m.CourseId == courseId)
                .ToListAsync();

            return Ok(modules);
        }

         
        // GET: Get single module by id
         
        [HttpGet("{id}")]
        public async Task<IActionResult> GetModule(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null) return NotFound();
            return Ok(module);
        }

         
        // POST: Create a new module (Admin only)
         
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateModule([FromBody] Module module)
        {
            _context.Modules.Add(module);
            await _context.SaveChangesAsync();
            return Ok(module);
        }

         
        // PUT: Update module details (Admin only)
         
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateModule(int id, [FromBody] Module updatedModule)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null) return NotFound();

            module.Title = updatedModule.Title;
            module.Content = updatedModule.Content;
            module.VideoUrl = updatedModule.VideoUrl;
            module.FileUrl = updatedModule.FileUrl;

            await _context.SaveChangesAsync();
            return Ok(module);
        }

         
        // DELETE: Remove module (Admin only)
         
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null) return NotFound();

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
            return Ok("Module deleted successfully.");
        }

         
        // POST: Upload file/video for a module (Admin only)
         
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/upload")]
        public async Task<IActionResult> UploadFile(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var module = await _context.Modules.FindAsync(id);
            if (module == null) return NotFound();

            // Upload via BlobServices (currently local storage backed)
            string fileUrl;
            using (var stream = file.OpenReadStream())
            {
                fileUrl = await _blobServices.UploadAsync(stream, file.FileName, "uploads");
            }

            if (file.ContentType.StartsWith("video/"))
                module.VideoUrl = fileUrl;
            else
                module.FileUrl = fileUrl;

            await _context.SaveChangesAsync();
            return Ok(new { module.Id, module.FileUrl, module.VideoUrl });
        }
    }
}