using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Models.Resources_Repo;
using System.Threading.Tasks;
using System.Linq;

namespace UnganaConnect.Controllers
{
    [Authorize]
    public class ResourceMvcController : Controller
    {
        private readonly UnganaConnectDbcontext _context;

        public ResourceMvcController(UnganaConnectDbcontext context)
        {
            _context = context;
        }

        // GET: /ResourceMvc
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var resources = await _context.Resources.ToListAsync();
            return View(resources);
        }

        // GET: /ResourceMvc/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources
                .FirstOrDefaultAsync(m => m.Id == id);
            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        // GET: /ResourceMvc/Upload
        [Authorize(Roles = "Staff,Admin")]
        public IActionResult Upload()
        {
            return View();
        }

        // POST: /ResourceMvc/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Upload([Bind("Title,FileUrl,Category")] CResource resource)
        {
            if (ModelState.IsValid)
            {
                _context.Add(resource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(resource);
        }

        // GET: /ResourceMvc/Stats
        [Authorize(Roles = "Staff,Admin")]
        public IActionResult Stats()
        {
            // Placeholder for stats view
            return View();
        }
    }
}
