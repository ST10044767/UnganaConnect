using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Frontend.Models;
using UnganaConnect.Frontend.Services;

namespace UnganaConnect.Frontend.Controllers
{
    public class ResourceController : Controller
    {
        private readonly ApiService _apiService;

        public ResourceController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string search = "", string category = "all")
        {
            var resources = GetSampleResources();
            var categories = new List<string> { "all", "Fundraising", "Finance", "Community", "Marketing", "Management", "Analytics" };
            var types = new List<string> { "all", "pdf", "video", "template", "toolkit", "guide" };
            
            var filteredResources = resources.Where(r => 
                (string.IsNullOrEmpty(search) || r.Title.Contains(search, StringComparison.OrdinalIgnoreCase) || 
                 r.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                 r.Tags.Any(tag => tag.Contains(search, StringComparison.OrdinalIgnoreCase))) &&
                (category == "all" || r.Category == category)
            ).ToList();

            var model = new ResourceLibraryViewModel
            {
                Resources = filteredResources,
                Categories = categories,
                Types = types,
                SearchTerm = search,
                SelectedCategory = category,
                Stats = new ResourceStatsViewModel
                {
                    TotalResources = filteredResources.Count,
                    TotalDownloads = filteredResources.Sum(r => r.Downloads),
                    ThisMonth = 12,
                    Contributors = 24
                }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Download(int id)
        {
            // Simulate successful download without backend
            var resource = _resources.FirstOrDefault(r => r.Id == id);
            if (resource != null)
            {
                resource.Downloads++;
                TempData["Success"] = "Resource downloaded successfully!";
            }
            else
            {
                TempData["Error"] = "Resource not found.";
            }

            return RedirectToAction("Index");
        }

        private static List<ResourceViewModel> _resources = new List<ResourceViewModel>
        {
            new() { Id = 1, Title = "Grant Proposal Template Package", Description = "Complete set of templates for writing successful grant proposals, including budget templates and evaluation frameworks.", Type = "template", Category = "Fundraising", Size = "2.4 MB", Downloads = 1247, UploadDate = DateTime.Parse("2024-12-15"), Tags = new List<string> { "grants", "templates", "fundraising" }, Author = "Dr. Sarah Williams" },
            new() { Id = 2, Title = "Financial Management Toolkit", Description = "Comprehensive toolkit covering budgeting, financial reporting, and compliance for NGOs.", Type = "toolkit", Category = "Finance", Size = "15.8 MB", Downloads = 892, UploadDate = DateTime.Parse("2024-12-10"), Tags = new List<string> { "finance", "budgeting", "compliance" }, Author = "Michael Chen" },
            new() { Id = 3, Title = "Community Engagement Best Practices", Description = "Video series showcasing successful community engagement strategies from leading CSOs.", Type = "video", Category = "Community", Size = "45.2 MB", Downloads = 634, UploadDate = DateTime.Parse("2024-12-08"), Tags = new List<string> { "community", "engagement", "best practices" }, Author = "Dr. Priya Patel" },
            new() { Id = 4, Title = "Digital Marketing Playbook", Description = "Step-by-step guide to building your online presence and reaching supporters through digital channels.", Type = "guide", Category = "Marketing", Size = "8.3 MB", Downloads = 1156, UploadDate = DateTime.Parse("2024-12-05"), Tags = new List<string> { "marketing", "digital", "social media" }, Author = "Emma Rodriguez" },
            new() { Id = 5, Title = "Project Planning Templates", Description = "Ready-to-use project planning templates including Gantt charts, risk assessments, and milestone trackers.", Type = "template", Category = "Management", Size = "3.7 MB", Downloads = 756, UploadDate = DateTime.Parse("2024-12-01"), Tags = new List<string> { "project management", "planning", "templates" }, Author = "James Thompson" },
            new() { Id = 6, Title = "Impact Measurement Framework", Description = "Comprehensive guide to measuring and reporting on your organization's social impact.", Type = "guide", Category = "Analytics", Size = "6.1 MB", Downloads = 423, UploadDate = DateTime.Parse("2024-11-28"), Tags = new List<string> { "impact", "measurement", "reporting" }, Author = "Alex Kim" }
        };

        private List<ResourceViewModel> GetSampleResources()
        {
            return _resources;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied. Admin or Instructor role required.";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResourceViewModel model, string tagsInput)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Parse tags from comma-separated string
            if (!string.IsNullOrEmpty(tagsInput))
            {
                model.Tags = tagsInput.Split(',')
                    .Select(tag => tag.Trim())
                    .Where(tag => !string.IsNullOrEmpty(tag))
                    .ToList();
            }

            // Simulate successful resource creation without backend
            model.Id = _resources.Count + 1;
            model.UploadDate = DateTime.Now;
            model.Downloads = 0;
            model.CreatedAt = DateTime.Now;

            _resources.Add(model);

            TempData["Success"] = "Resource created successfully!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var resource = _resources.FirstOrDefault(r => r.Id == id);
            if (resource == null)
                return NotFound();

            return View(resource);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied. Admin or Instructor role required.";
                return RedirectToAction("Index");
            }

            var resource = _resources.FirstOrDefault(r => r.Id == id);
            if (resource == null)
                return NotFound();

            return View(resource);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ResourceViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resource = _resources.FirstOrDefault(r => r.Id == id);
            if (resource == null)
                return NotFound();

            // Update the resource properties
            resource.Title = model.Title;
            resource.Description = model.Description;
            resource.Type = model.Type;
            resource.Category = model.Category;
            resource.Size = model.Size;
            resource.Tags = model.Tags;
            resource.Author = model.Author;

            TempData["Success"] = "Resource updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied. Admin or Instructor role required.";
                return RedirectToAction("Index");
            }

            var resource = _resources.FirstOrDefault(r => r.Id == id);
            if (resource != null)
            {
                _resources.Remove(resource);
                TempData["Success"] = "Resource deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Resource not found";
            }

            return RedirectToAction("Index");
        }
    }
}