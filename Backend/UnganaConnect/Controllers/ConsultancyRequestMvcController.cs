using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Models.Consultancy_Management;

namespace UnganaConnect.Controllers
{
    public class ConsultancyRequestMvcController : Controller
    {
        private readonly UnganaConnectDbcontext _context;

        public ConsultancyRequestMvcController(UnganaConnectDbcontext context)
        {
            _context = context;
        }

        // GET: ConsultancyRequestMvc/Index
        public async Task<IActionResult> Index()
        {
            var requests = await _context.ConsultancyRequests
                .Include(c => c.User)
                .Include(c => c.AssignedStaff)
                .ToListAsync();
            return View(requests);
        }

        // GET: ConsultancyRequestMvc/MyRequests
        [Authorize(Roles = "CSO")]
        public async Task<IActionResult> MyRequests()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var requests = await _context.ConsultancyRequests
                .Where(c => c.UserId == userId)
                .Include(c => c.AssignedStaff)
                .ToListAsync();
            return View(requests);
        }

        // GET: ConsultancyRequestMvc/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultancyRequest = await _context.ConsultancyRequests
                .Include(c => c.User)
                .Include(c => c.AssignedStaff)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (consultancyRequest == null)
            {
                return NotFound();
            }

            return View(consultancyRequest);
        }

        // GET: ConsultancyRequestMvc/Create
        [Authorize(Roles = "CSO")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ConsultancyRequestMvc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CSO")]
        public async Task<IActionResult> Create([Bind("Title,Description,Priority")] ConsultancyRequest consultancyRequest)
        {
            if (ModelState.IsValid)
            {
                consultancyRequest.UserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                consultancyRequest.Status = "Pending";
                consultancyRequest.CreatedDate = DateTime.Now;
                _context.Add(consultancyRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyRequests));
            }
            return View(consultancyRequest);
        }

        // GET: ConsultancyRequestMvc/Assign/5
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Assign(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultancyRequest = await _context.ConsultancyRequests
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (consultancyRequest == null)
            {
                return NotFound();
            }

            ViewBag.Staff = await _context.Users.Where(u => u.Role == "Staff").ToListAsync();
            return View(consultancyRequest);
        }

        // POST: ConsultancyRequestMvc/Assign/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Assign(int id, int assignedStaffId)
        {
            var consultancyRequest = await _context.ConsultancyRequests.FindAsync(id);
            if (consultancyRequest == null)
            {
                return NotFound();
            }

            consultancyRequest.AssignedStaffId = assignedStaffId;
            consultancyRequest.Status = "Assigned";
            _context.Update(consultancyRequest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
