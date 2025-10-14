using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Models.Training___Learning;

namespace UnganaConnect.Controllers.Course
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModuleController : ControllerBase
    {
        private readonly UnganaConnectDbcontext _context;

        public ModuleController(UnganaConnectDbcontext context)
        {
            _context = context;
        }

        // GET all modules for a course
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetModulesByCourse(int courseId)
        {
            var modules = await _context.Modules
                .Where(m => m.CourseId == courseId)
                .OrderBy(m => m.Id)
                .ToListAsync();

            return Ok(modules);
        }

        // GET single module
        [HttpGet("{id}")]
        public async Task<IActionResult> GetModule(int id)
        {
            var module = await _context.Modules
                .Include(m => m.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (module == null)
                return NotFound("Module not found.");

            return Ok(module);
        }

        // CREATE module (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateModule([FromBody] Module module)
        {
            _context.Modules.Add(module);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetModule), new { id = module.Id }, module);
        }

        // UPDATE module (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateModule(int id, [FromBody] Module updatedModule)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
                return NotFound("Module not found.");

            module.Title = updatedModule.Title;
            module.Content = updatedModule.Content;
            module.VideoUrl = updatedModule.VideoUrl;
            module.FileUrl = updatedModule.FileUrl;

            await _context.SaveChangesAsync();
            return Ok(module);
        }

        // DELETE module (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
                return NotFound("Module not found.");

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}