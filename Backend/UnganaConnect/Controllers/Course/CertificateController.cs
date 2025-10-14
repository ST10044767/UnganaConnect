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
    public class CertificateController : ControllerBase
    {
        private readonly UnganaConnectDbcontext _context;
        private readonly CertificateService _certificateService;

        public CertificateController(UnganaConnectDbcontext context, CertificateService certificateService)
        {
            _context = context;
            _certificateService = certificateService;
        }

        // GET user's certificates
        [Authorize]
        [HttpGet("my-certificates")]
        public async Task<IActionResult> GetUserCertificates()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var certificates = await _context.Certificates
                .Include(c => c.Course)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return Ok(certificates);
        }

        // GENERATE certificate for completed course
        [Authorize]
        [HttpPost("generate/{courseId}")]
        public async Task<IActionResult> GenerateCertificate(int courseId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            // Check if user is enrolled and completed the course
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId && e.IsCompleted);

            if (enrollment == null)
                return BadRequest("Course not completed or user not enrolled.");

            // Check if certificate already exists
            var existingCertificate = await _context.Certificates
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId);

            if (existingCertificate != null)
                return BadRequest("Certificate already exists for this course.");

            // Get user details
            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
                return NotFound("User not found.");

            // Generate certificate PDF
            var userName = $"{user.FirstName} {user.LastName}";
            var pdfBytes = _certificateService.GenerateCertificatePdf(
                userId, 
                userName, 
                enrollment.Course.Title, 
                DateTime.UtcNow
            );

            // Save certificate to database
            var certificate = new Certificate
            {
                UserId = userId,
                CourseId = courseId,
                IssuedOn = DateTime.UtcNow,
                FileUrl = $"certificates/{userId}_{courseId}_{DateTime.UtcNow:yyyyMMdd}.pdf"
            };

            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();

            // Return PDF file
            return File(pdfBytes, "application/pdf", $"Certificate_{enrollment.Course.Title}_{userName}.pdf");
        }

        // DOWNLOAD existing certificate
        [Authorize]
        [HttpGet("download/{certificateId}")]
        public async Task<IActionResult> DownloadCertificate(int certificateId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var certificate = await _context.Certificates
                .Include(c => c.Course)
                .FirstOrDefaultAsync(c => c.Id == certificateId && c.UserId == userId);

            if (certificate == null)
                return NotFound("Certificate not found.");

            // Get user details
            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
                return NotFound("User not found.");

            // Regenerate certificate PDF
            var userName = $"{user.FirstName} {user.LastName}";
            var pdfBytes = _certificateService.GenerateCertificatePdf(
                userId,
                userName,
                certificate.Course.Title,
                certificate.IssuedOn
            );

            return File(pdfBytes, "application/pdf", $"Certificate_{certificate.Course.Title}_{userName}.pdf");
        }

        // GET all certificates (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCertificates()
        {
            var certificates = await _context.Certificates
                .Include(c => c.Course)
                .Include(c => c.User)
                .OrderByDescending(c => c.IssuedOn)
                .ToListAsync();

            return Ok(certificates);
        }

        // GET certificate statistics (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpGet("statistics")]
        public async Task<IActionResult> GetCertificateStatistics()
        {
            var totalCertificates = await _context.Certificates.CountAsync();
            var certificatesThisMonth = await _context.Certificates
                .CountAsync(c => c.IssuedOn.Month == DateTime.UtcNow.Month && c.IssuedOn.Year == DateTime.UtcNow.Year);

            var certificatesByCourse = await _context.Certificates
                .Include(c => c.Course)
                .GroupBy(c => c.Course.Title)
                .Select(g => new
                {
                    CourseTitle = g.Key,
                    CertificateCount = g.Count()
                })
                .OrderByDescending(x => x.CertificateCount)
                .ToListAsync();

            return Ok(new
            {
                TotalCertificates = totalCertificates,
                CertificatesThisMonth = certificatesThisMonth,
                CertificatesByCourse = certificatesByCourse
            });
        }
    }
}
