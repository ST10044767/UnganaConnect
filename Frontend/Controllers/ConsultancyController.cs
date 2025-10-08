using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Frontend.Models;
using UnganaConnect.Frontend.Services;

namespace UnganaConnect.Frontend.Controllers
{
    public class ConsultancyController : Controller
    {
        private readonly ApiService _apiService;

        public ConsultancyController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            var model = new ConsultancyIndexViewModel
            {
                MyRequests = GetMyRequests(),
                AvailableConsultants = GetAvailableConsultants(),
                ConsultancyAreas = GetConsultancyAreas()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(CreateConsultancyRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields.";
                return RedirectToAction("Index");
            }

            var token = HttpContext.Session.GetString("Token");
            var response = await _apiService.PostAsync("consultancy/requests", model, token);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Consultancy request submitted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to submit request.";
            }

            return RedirectToAction("Index");
        }

        public IActionResult RequestDetails(int id)
        {
            var request = GetMyRequests().FirstOrDefault(r => r.Id == id);
            if (request == null)
                return NotFound();

            return View(request);
        }

        private List<ConsultancyRequestViewModel> GetMyRequests()
        {
            return new List<ConsultancyRequestViewModel>
            {
                new() { Id = 1, Title = "Grant Writing Support for Education Project", Area = "Grant Writing & Fundraising", Status = "In Progress", Consultant = "Dr. Amara Okafor", Submitted = DateTime.Parse("2024-03-01"), Deadline = DateTime.Parse("2024-03-15"), Priority = "High", Description = "Need assistance writing a comprehensive grant proposal for a community education initiative targeting rural schools in Kenya." },
                new() { Id = 2, Title = "Financial Management System Setup", Area = "Financial Management", Status = "Completed", Consultant = "Sarah Mensah", Submitted = DateTime.Parse("2024-02-15"), Deadline = DateTime.Parse("2024-02-28"), Priority = "Medium", Description = "Implementing a new financial management system and training staff on proper bookkeeping procedures." },
                new() { Id = 3, Title = "Digital Marketing Strategy Review", Area = "Digital Marketing", Status = "Pending Review", Consultant = "Unassigned", Submitted = DateTime.Parse("2024-03-10"), Deadline = DateTime.Parse("2024-03-25"), Priority = "Low", Description = "Review and optimize our current digital marketing approach to increase donor engagement and visibility." }
            };
        }

        private List<ConsultantViewModel> GetAvailableConsultants()
        {
            return new List<ConsultantViewModel>
            {
                new() { Name = "Dr. Amara Okafor", Expertise = new List<string> { "Grant Writing", "Fundraising", "Strategic Planning" }, Experience = "15+ years", Rating = 4.9, Location = "Nigeria", Languages = new List<string> { "English", "Yoruba", "French" } },
                new() { Name = "Sarah Mensah", Expertise = new List<string> { "Financial Management", "Accounting", "Compliance" }, Experience = "12+ years", Rating = 4.8, Location = "Ghana", Languages = new List<string> { "English", "Twi" } },
                new() { Name = "David Kiptoo", Expertise = new List<string> { "Project Management", "Capacity Building", "Training" }, Experience = "10+ years", Rating = 4.7, Location = "Kenya", Languages = new List<string> { "English", "Swahili" } }
            };
        }

        private List<string> GetConsultancyAreas()
        {
            return new List<string>
            {
                "Grant Writing & Fundraising",
                "Financial Management",
                "Strategic Planning",
                "Project Management",
                "Digital Marketing",
                "Legal Compliance",
                "Capacity Building",
                "Impact Measurement",
                "Partnership Development",
                "Technology Implementation"
            };
        }
    }
}