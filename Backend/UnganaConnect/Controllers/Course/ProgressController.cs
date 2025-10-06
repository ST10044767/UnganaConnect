using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Models.Training___Learning;
using UnganaConnect.Service;

namespace UnganaConnect.Controllers.Course
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressController : ControllerBase
    {
        private readonly UnganaConnectDbcontext _context;
        private readonly BlobServices _blobServices;
        private readonly CertificateService _certificateService;

        public ProgressController(UnganaConnectDbcontext context, BlobServices blobServices, CertificateService certificateService)
        {
            _context = context;
            _blobServices = blobServices;
            _certificateService = certificateService;
        }

        // Mark a module as completed by a user
        [Authorize]
        [HttpPost("module/{moduleId}/complete/{userId}")]
        public async Task<IActionResult> CompleteModule(int moduleId, string userId)
        {
            var existing = await _context.UserProgress
                .FirstOrDefaultAsync(p => p.ModuleId == moduleId && p.UserId == userId);
            if (existing == null)
            {
                _context.UserProgress.Add(new UserProgress
                {
                    ModuleId = moduleId,
                    UserId = userId,
                    IsCompleted = true
                });
                await _context.SaveChangesAsync();
            }

            // After marking complete, check if course is fully completed
            var module = await _context.Modules.FirstOrDefaultAsync(m => m.Id == moduleId);
            if (module == null) return NotFound("Module not found");

            var courseId = module.CourseId;
            var totalModules = await _context.Modules.CountAsync(m => m.CourseId == courseId);
            var completedModules = await _context.UserProgress
                .Where(p => p.UserId == userId && p.IsCompleted)
                .Join(_context.Modules, p => p.ModuleId, m => m.Id, (p, m) => m)
                .CountAsync(m => m.CourseId == courseId);

            if (totalModules > 0 && completedModules == totalModules)
            {
                // Issue certificate if not already issued
                var existingCert = await _context.Certificates
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId);
                if (existingCert == null)
                {
                    // Generate PDF certificate
                    var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
                    var userName = userId; // could be replaced by actual user name lookup
                    var bytes = _certificateService.GenerateCertificatePdf(userId, userName, course?.Title ?? $"Course #{courseId}", DateTime.UtcNow);
                    await using var ms = new MemoryStream(bytes);
                    var certUrl = await _blobServices.UploadAsync(ms, $"certificate_{userId}_{courseId}.pdf", "certificates");

                    var newCert = new Certificate
                    {
                        UserId = userId,
                        CourseId = courseId,
                        IssuedOn = DateTime.UtcNow,
                        FileUrl = certUrl
                    };
                    _context.Certificates.Add(newCert);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new { message = "Module completion recorded." });
        }

        // Get certificate by user and course
        [Authorize]
        [HttpGet("certificate/{courseId}/user/{userId}")]
        public async Task<IActionResult> GetCertificate(int courseId, string userId)
        {
            var cert = await _context.Certificates.FirstOrDefaultAsync(c => c.CourseId == courseId && c.UserId == userId);
            if (cert == null) return NotFound();
            return Ok(cert);
        }

        // Download certificate file by certificate id (redirect to URL if hosted)
        [Authorize]
        [HttpGet("certificate/{certificateId}/download")]
        public async Task<IActionResult> DownloadCertificate(int certificateId)
        {
            var cert = await _context.Certificates.FirstOrDefaultAsync(c => c.Id == certificateId);
            if (cert == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(cert.FileUrl))
            {
                // If stored in Azure or public path, just redirect
                return Redirect(cert.FileUrl);
            }
            return NotFound("Certificate file not available.");
        }
    }
}


