using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Frontend.Models;
using UnganaConnect.Frontend.Services;
using System.Collections.Generic;

namespace UnganaConnect.Frontend.Controllers
{
    public class ConsultancyController : Controller
    {
        private readonly ApiService _apiService;
        private static List<ConsultancyRequestViewModel> _requests = new();

        public ConsultancyController(ApiService apiService)
        {
            _apiService = apiService;
            if (_requests.Count == 0)
            {
                _requests = GetMyRequests();
            }
        }

        [Authorize]
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            var model = new ConsultancyIndexViewModel
            {
                MyRequests = role == "Admin" ? _requests : GetMyRequests(),
                AvailableConsultants = GetAvailableConsultants(),
                ConsultancyAreas = GetConsultancyAreas()
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRequest(CreateConsultancyRequestViewModel model)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Error"] = "Please log in to create consultancy requests.";
                return RedirectToAction("Index", "Auth");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields.";
                return RedirectToAction("Index");
            }

            // Simulate successful submission without calling the backend API
            TempData["Success"] = "Consultancy request submitted successfully!";

            // Add the new request to the list (in a real app, this would be saved to database)
            var newRequest = new ConsultancyRequestViewModel
            {
                Id = _requests.Max(r => r.Id) + 1,
                Title = model.Title,
                Area = model.Area,
                Status = "Pending Review",
                Consultant = "Unassigned",
                Submitted = DateTime.Now,
                Deadline = model.Deadline,
                Priority = model.Priority,
                Description = model.Description,
                BudgetRange = model.BudgetRange
            };

            // Add to the static list
            _requests.Add(newRequest);

            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult RequestDetails(int id)
        {
            var request = GetMyRequests().FirstOrDefault(r => r.Id == id);
            if (request == null)
                return NotFound();

            return View(request);
        }

        [Authorize]
        public IActionResult ConsultantProfile(int id)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Error"] = "Please log in to view consultant profiles.";
                return RedirectToAction("Index", "Auth");
            }

            var consultant = GetAvailableConsultants().FirstOrDefault(c => c.Id == id);
            if (consultant == null)
                return NotFound();

            return View(consultant);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RequestConsultation(int consultantId, string title, string area, string priority, DateTime deadline, string description, string budgetRange)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Error"] = "Please log in to request consultations.";
                return RedirectToAction("Index", "Auth");
            }

            // Simulate successful consultation request
            TempData["Success"] = "Consultation request sent successfully!";

            // Create a new request for this consultation
            var newRequest = new ConsultancyRequestViewModel
            {
                Id = _requests.Max(r => r.Id) + 1,
                Title = title,
                Area = area,
                Status = "Pending Review",
                Consultant = GetAvailableConsultants().FirstOrDefault(c => c.Id == consultantId)?.Name ?? "Unassigned",
                Submitted = DateTime.Now,
                Deadline = deadline,
                Priority = priority,
                Description = description,
                BudgetRange = budgetRange
            };

            _requests.Add(newRequest);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AssignConsultant(int requestId, string consultantName)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return RedirectToAction("Index");

            var request = _requests.FirstOrDefault(r => r.Id == requestId);
            if (request == null)
                return NotFound();

            request.Consultant = consultantName;
            request.Status = "Assigned";
            TempData["Success"] = "Consultant assigned successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRequestStatus(int requestId, string status)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return RedirectToAction("Index");

            var request = _requests.FirstOrDefault(r => r.Id == requestId);
            if (request == null)
                return NotFound();

            request.Status = status;
            TempData["Success"] = $"Request status updated to {status}!";
            return RedirectToAction("Index");
        }

        private List<ConsultancyRequestViewModel> GetMyRequests()
        {
            var defaultRequests = new List<ConsultancyRequestViewModel>
            {
                new() { Id = 1, Title = "Grant Writing Support for Education Project", Area = "Grant Writing & Fundraising", Status = "In Progress", Consultant = "Dr. Amara Okafor", Submitted = DateTime.Parse("2024-03-01"), Deadline = DateTime.Parse("2024-03-15"), Priority = "High", Description = "Need assistance writing a comprehensive grant proposal for a community education initiative targeting rural schools in Kenya." },
                new() { Id = 2, Title = "Financial Management System Setup", Area = "Financial Management", Status = "Completed", Consultant = "Sarah Mensah", Submitted = DateTime.Parse("2024-02-15"), Deadline = DateTime.Parse("2024-02-28"), Priority = "Medium", Description = "Implementing a new financial management system and training staff on proper bookkeeping procedures." },
                new() { Id = 3, Title = "Digital Marketing Strategy Review", Area = "Digital Marketing", Status = "Pending Review", Consultant = "Unassigned", Submitted = DateTime.Parse("2024-03-10"), Deadline = DateTime.Parse("2024-03-25"), Priority = "Low", Description = "Review and optimize our current digital marketing approach to increase donor engagement and visibility." }
            };

            // Combine default requests with dynamically added ones
            return defaultRequests.Concat(_requests).ToList();
        }

        private List<ConsultantViewModel> GetAvailableConsultants()
        {
            return new List<ConsultantViewModel>
            {
                new() { Id = 1, Name = "Dr. Amara Okafor", Expertise = new List<string> { "Grant Writing", "Fundraising", "Strategic Planning" }, Experience = "15+ years", Rating = 4.9, Location = "Nigeria", Languages = new List<string> { "English", "Yoruba", "French" } },
                new() { Id = 2, Name = "Sarah Mensah", Expertise = new List<string> { "Financial Management", "Accounting", "Compliance" }, Experience = "12+ years", Rating = 4.8, Location = "Ghana", Languages = new List<string> { "English", "Twi" } },
                new() { Id = 3, Name = "David Kiptoo", Expertise = new List<string> { "Project Management", "Capacity Building", "Training" }, Experience = "10+ years", Rating = 4.7, Location = "Kenya", Languages = new List<string> { "English", "Swahili" } }
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