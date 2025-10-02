using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Models.Training___Learning;

namespace UnganaConnect.Controllers
{
    public class CertificateMvcController : Controller
    {
        private readonly UnganaConnectDbcontext _context;

        public CertificateMvcController(UnganaConnectDbcontext context)
        {
            _context = context;
        }

        // GET: /CertificateMvc
        public async Task<IActionResult> Index()
        {
            var certificates = await _context.Certificates.ToListAsync();
            return View(certificates);
        }

        // GET: /CertificateMvc/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificate = await _context.Certificates.FirstOrDefaultAsync(m => m.Id == id);
            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificate);
        }
    }
}
