using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Frontend.Models;

namespace UnganaConnect.Frontend.Controllers
{
    public class EventController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new EventViewModel
            {
                UpcomingEvents = GetUpcomingEvents(),
                MyEvents = GetMyEvents()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Register(int eventId)
        {
            // TODO: Register user for event
            TempData["Success"] = "Successfully registered for event!";
            return RedirectToAction("Index");
        }

        private List<Event> GetUpcomingEvents()
        {
            return new List<Event>
            {
                new() { Id = 1, Title = "Grant Writing Masterclass for African CSOs", Type = "Workshop", Format = "Virtual", Date = "2024-03-20", Time = "14:00 - 17:00 GMT", Duration = "3 hours", Instructor = "Dr. Amara Okafor", Participants = 45, MaxParticipants = 50, Price = "Free", Level = "Intermediate", Description = "Learn advanced grant writing techniques specifically tailored for African CSOs. Cover proposal structure, budget planning, and impact measurement.", Agenda = new() { "Understanding donor priorities in Africa", "Crafting compelling narratives", "Budget development and justification", "Impact measurement frameworks", "Q&A and feedback session" }, Materials = new() { "Workbook PDF", "Template Library", "Recording Access" }, Tags = new() { "Grant Writing", "Fundraising", "Capacity Building" } },
                new() { Id = 2, Title = "CSO Leadership Summit 2025", Type = "Conference", Format = "Hybrid", Date = "2024-04-15", Time = "09:00 - 17:00 GMT", Duration = "2 days", Instructor = "Multiple Speakers", Participants = 120, MaxParticipants = 200, Price = "R75", Level = "All Levels", Location = "Johannesburg, South Africa", Description = "Annual summit bringing together CSO leaders from across Africa to share insights, network, and learn about emerging trends.", Agenda = new() { "Opening keynote: Future of Civil Society", "Panel: Digital Transformation for CSOs", "Workshop: Sustainable Financing Models", "Networking lunch", "Case studies: Successful CSO innovations", "Closing ceremony and awards" }, Materials = new() { "Conference Kit", "Networking Directory", "Session Recordings" }, Tags = new() { "Leadership", "Networking", "Innovation" } },
                new() { Id = 3, Title = "Financial Management for Small CSOs", Type = "Webinar", Format = "Virtual", Date = "2024-03-25", Time = "15:00 - 16:30 GMT", Duration = "1.5 hours", Instructor = "Sarah Mensah", Participants = 28, MaxParticipants = 100, Price = "Free", Level = "Beginner", Description = "Essential financial management practices for small and emerging CSOs, including bookkeeping, reporting, and compliance.", Agenda = new() { "Basic financial management principles", "Setting up simple accounting systems", "Donor reporting requirements", "Cash flow management", "Live Q&A session" }, Materials = new() { "Financial Templates", "Checklist", "Resource Links" }, Tags = new() { "Finance", "Compliance", "Management" } }
            };
        }

        private List<MyEvent> GetMyEvents()
        {
            return new List<MyEvent>
            {
                new() { Id = 1, Title = "Grant Writing Masterclass for African CSOs", Date = "2024-03-20", Status = "Registered", Type = "Workshop" },
                new() { Id = 2, Title = "Digital Marketing for CSOs", Date = "2024-02-28", Status = "Completed", Type = "Webinar", Rating = 4.5 },
                new() { Id = 3, Title = "Partnership Building Strategies", Date = "2024-03-30", Status = "Waitlisted", Type = "Workshop" }
            };
        }
    }
}