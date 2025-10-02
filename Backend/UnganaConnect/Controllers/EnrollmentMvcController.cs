using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Models.Training___Learning;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace UnganaConnect.Controllers
{
    public class EnrollmentMvcController : Controller
    {
        private readonly UnganaConnectDbcontext _context;

        public EnrollmentMvcController(UnganaConnectDbcontext context)
        {
            _context = context;
        }

        // GET: /EnrollmentMvc
        public async Task<IActionResult> Index()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .ToListAsync();
            return View(enrollments);
        }

        // GET: /EnrollmentMvc/MyLearning
        [Authorize]
        public async Task<IActionResult> MyLearning()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = userIdClaim.Value;
            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.UserId == userId)
                .ToListAsync();
            return View(enrollments);
        }

        // GET: /EnrollmentMvc/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: /EnrollmentMvc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /EnrollmentMvc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,CourseId")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(enrollment);
        }

        // GET: /EnrollmentMvc/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            return View(enrollment);
        }

        // POST: /EnrollmentMvc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,CourseId,EnrolledOn,IsActive")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(enrollment);
        }

        // GET: /EnrollmentMvc/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: /EnrollmentMvc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.Id == id);
        }
    }
}
