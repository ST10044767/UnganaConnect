using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Frontend.Models;
using UnganaConnect.Models;
using UnganaConnect.Models.Resource;
using UnganaConnect.Frontend.Services;


namespace UnganaConnect.Controllers
{
    public class ResourceController : Controller
    {
        private readonly UnganaConnectDbContext _context;
        private readonly AzureBlobService _blobService;

        public ResourceController(UnganaConnectDbContext context, AzureBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }


        public async Task<IActionResult> Index(string search = "", string category = "all")
        {
            var query = _context.Resources.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(r => r.Title.Contains(search) || r.Description.Contains(search) || r.Tags.Contains(search));

            if (category != "all")
                query = query.Where(r => r.Category == category);

            var resources = await query.OrderByDescending(r => r.UploadDate).ToListAsync();

            var viewModel = new ResourceLibraryViewModel
            {
                Resources = resources,
                Categories = new List<string> { "all","ICT's", "Fundraising", "Finance", "Community", "Marketing", "Management", "Analytics" },
                SelectedCategory = category,
                SearchTerm = search
            };

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Resource model, IFormFile file, string tagsInput)
        {
            if (!ModelState.IsValid || file == null)
            {
                TempData["Error"] = "Please fill all required fields and upload a file.";
                return View(model);
            }

            var fileUrl = await _blobService.FileUploadAsync(file);

            model.Url = fileUrl;
            model.UploadDate = DateTime.UtcNow; // use UTC
            model.CreatedAt = DateTime.UtcNow;  // use UTC
            model.Tags = tagsInput ?? string.Empty;

            _context.Resources.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Resource uploaded successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null) return NotFound();
            return View(resource);
        }

       

        // =============================
        // EDIT
        // =============================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction("Index");
            }

            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
                return NotFound();

            return View(resource);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Resource model, IFormFile? newFile, string tagsInput)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null) return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            resource.Title = model.Title;
            resource.Description = model.Description;
            resource.Type = model.Type;
            resource.Category = model.Category;
            resource.Tags = tagsInput ?? model.Tags;
            

            // If a new file is uploaded, replace it
            if (newFile != null)
            {
                await _blobService.DeleteAsync(resource.Url);
                var newUrl = await _blobService.FileUploadAsync(newFile);
                resource.Url = newUrl;
                resource.UploadDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Resource updated successfully!";
            return RedirectToAction("Index");
        }

        // =============================
        // DOWNLOAD
        // =============================
        [HttpPost]
        public async Task<IActionResult> Download(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
            {
                TempData["Error"] = "Resource not found.";
                return RedirectToAction("Index");
            }

            // Increment download counter
            resource.Downloads += 1;
            await _context.SaveChangesAsync();

            // Redirect user to Azure Blob file URL (direct download)
            return Redirect(resource.Url);
        }

        // =============================
        // DELETE
        // =============================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction("Index");
            }

            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
            {
                TempData["Error"] = "Resource not found.";
                return RedirectToAction("Index");
            }

            await _blobService.DeleteAsync(resource.Url);
            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Resource deleted successfully!";
            return RedirectToAction("Index");
        }

        // =============================
        // STATS (Optional Admin Dashboard)
        // =============================
        [HttpGet]
        public async Task<IActionResult> Stats()
        {
            var total = await _context.Resources.CountAsync();
            var totalDownloads = await _context.Resources.SumAsync(r => r.Downloads);
            var thisMonth = await _context.Resources.CountAsync(r => r.UploadDate.Month == DateTime.Now.Month);
          

            var stats = new
            {
                TotalResources = total,
                TotalDownloads = totalDownloads,
                UploadedThisMonth = thisMonth,
               
            };

            return Json(stats);
        }
    }
}


