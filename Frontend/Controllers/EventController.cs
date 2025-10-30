using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Frontend.Models;
using System.Text;

namespace UnganaConnect.Frontend.Controllers
{
    public class EventController : Controller
    {
        private static List<Event> _events = new();
        private static List<EventRegistration> _registrations = new();

        public EventController()
        {
            // Initialize with sample data if empty
            if (!_events.Any())
            {
                _events = GetUpcomingEvents();
            }
        }

        public IActionResult Index()
        {
            var viewModel = new EventViewModel
            {
                UpcomingEvents = _events,
                MyEvents = GetMyEvents()
            };

            return View(viewModel);
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            var eventItem = _events.FirstOrDefault(e => e.Id == id);
            if (eventItem == null)
                return NotFound();

            return View(eventItem);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Register(int eventId)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Error"] = "Please log in to register for events.";
                return RedirectToAction("Index", "Auth");
            }

            var eventItem = _events.FirstOrDefault(e => e.Id == eventId);
            if (eventItem == null)
            {
                TempData["Error"] = "Event not found.";
                return RedirectToAction("Index");
            }

            // Check if already registered
            var existingRegistration = _registrations.FirstOrDefault(r => r.EventId == eventId && r.UserEmail == userEmail);
            if (existingRegistration != null)
            {
                if (existingRegistration.Status == "Registered")
                {
                    TempData["Info"] = "You are already registered for this event.";
                    return RedirectToAction("Join", new { id = eventId });
                }
                else if (existingRegistration.Status == "Completed")
                {
                    TempData["Info"] = "You have already completed this event.";
                    return RedirectToAction("Index");
                }
            }

            var model = new EventRegistrationViewModel
            {
                EventId = eventId,
                EventTitle = eventItem.Title,
                EventDate = eventItem.Date,
                EventTime = eventItem.Time,
                EventLocation = eventItem.Location,
                Price = eventItem.Price,
                Email = userEmail // Pre-fill email from session
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(EventRegistrationViewModel model)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Error"] = "Please log in to register for events.";
                return RedirectToAction("Index", "Auth");
            }

            // Re-populate event details for the view first
            var eventDetails = _events.FirstOrDefault(e => e.Id == model.EventId);
            if (eventDetails != null)
            {
                model.EventTitle = eventDetails.Title;
                model.EventDate = eventDetails.Date;
                model.EventTime = eventDetails.Time;
                model.EventLocation = eventDetails.Location;
                model.Price = eventDetails.Price;
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var eventItem = _events.FirstOrDefault(e => e.Id == model.EventId);
            if (eventItem == null)
            {
                TempData["Error"] = "Event not found.";
                return RedirectToAction("Index");
            }

            // Check if already registered
            if (_registrations.Any(r => r.EventId == model.EventId && r.UserEmail == userEmail))
            {
                TempData["Error"] = "You are already registered for this event.";
                return RedirectToAction("Index");
            }

            // Check capacity
            var currentRegistrations = _registrations.Count(r => r.EventId == model.EventId);
            if (currentRegistrations >= eventItem.MaxParticipants)
            {
                TempData["Error"] = "Event is full.";
                return RedirectToAction("Index");
            }

            // Register user
            _registrations.Add(new EventRegistration
            {
                EventId = model.EventId,
                UserEmail = userEmail,
                RegistrationDate = DateTime.Now,
                Status = "Registered"
            });

            TempData["Success"] = "Successfully registered for event! You will receive a confirmation email with the meeting link in a few minutes.";
            return RedirectToAction("Details", new { id = model.EventId });
        }

        [Authorize]
        public IActionResult Join(int id)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Error"] = "Please log in to join events.";
                return RedirectToAction("Index", "Auth");
            }

            var registration = _registrations.FirstOrDefault(r => r.EventId == id && r.UserEmail == userEmail);
            if (registration == null)
            {
                TempData["Error"] = "You are not registered for this event.";
                return RedirectToAction("Index");
            }

            // Simulate joining the event (in real app, this would redirect to meeting link)
            TempData["Success"] = "Joining event... (This would redirect to the virtual meeting in a real application)";
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult DownloadCertificate(int id)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Error"] = "Please log in to download certificates.";
                return RedirectToAction("Index", "Auth");
            }

            var registration = _registrations.FirstOrDefault(r => r.EventId == id && r.UserEmail == userEmail && r.Status == "Completed");
            if (registration == null)
            {
                TempData["Error"] = "Certificate not available. Event not completed.";
                return RedirectToAction("Index");
            }

            var eventItem = _events.FirstOrDefault(e => e.Id == id);
            if (eventItem == null)
            {
                TempData["Error"] = "Event not found.";
                return RedirectToAction("Index");
            }

            // Generate simple PDF-like certificate content
            var certificateContent = $@"
EVENT PARTICIPATION CERTIFICATE

This certifies that

{userEmail}

has successfully participated in the event

{eventItem.Title}

Event Date: {eventItem.Date}
Event Type: {eventItem.Type}
Instructor: {eventItem.Instructor}

Certificate ID: EVT-{id}-{userEmail.GetHashCode():X8}
Completion Date: {DateTime.Now:yyyy-MM-dd}

Ungana Connect Learning Platform
Digital Certificate
";

            var fileName = $"Event_Certificate_{id}_{DateTime.Now:yyyyMMdd}.txt";
            var bytes = Encoding.UTF8.GetBytes(certificateContent);

            return File(bytes, "text/plain", fileName);
        }

        // CRUD Operations for Admin/Instructor
        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied. Admin or Instructor role required.";
                return RedirectToAction("Index");
            }

            return View(new Event());
        }

        [HttpPost]
        public IActionResult Create(Event model)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied. Admin or Instructor role required.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                model.Id = _events.Max(e => e.Id) + 1;
                _events.Add(model);
                TempData["Success"] = "Event created successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied. Admin or Instructor role required.";
                return RedirectToAction("Index");
            }

            var eventItem = _events.FirstOrDefault(e => e.Id == id);
            if (eventItem == null)
                return NotFound();

            return View(eventItem);
        }

        [HttpPost]
        public IActionResult Edit(Event model)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied. Admin or Instructor role required.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                var eventItem = _events.FirstOrDefault(e => e.Id == model.Id);
                if (eventItem != null)
                {
                    // Update the event
                    eventItem.Title = model.Title;
                    eventItem.Type = model.Type;
                    eventItem.Format = model.Format;
                    eventItem.Date = model.Date;
                    eventItem.Time = model.Time;
                    eventItem.Duration = model.Duration;
                    eventItem.Instructor = model.Instructor;
                    eventItem.MaxParticipants = model.MaxParticipants;
                    eventItem.Price = model.Price;
                    eventItem.Level = model.Level;
                    eventItem.Location = model.Location;
                    eventItem.Description = model.Description;
                    eventItem.Agenda = model.Agenda;
                    eventItem.Materials = model.Materials;
                    eventItem.Tags = model.Tags;

                    TempData["Success"] = "Event updated successfully!";
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied. Admin or Instructor role required.";
                return RedirectToAction("Index");
            }

            var eventItem = _events.FirstOrDefault(e => e.Id == id);
            if (eventItem == null)
                return NotFound();

            return View(eventItem);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied. Admin or Instructor role required.";
                return RedirectToAction("Index");
            }

            var eventItem = _events.FirstOrDefault(e => e.Id == id);
            if (eventItem != null)
            {
                _events.Remove(eventItem);
                // Also remove all registrations for this event
                _registrations.RemoveAll(r => r.EventId == id);
                TempData["Success"] = "Event deleted successfully!";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Share(int id)
        {
            var eventItem = GetUpcomingEvents().FirstOrDefault(e => e.Id == id);
            if (eventItem == null)
                return NotFound();

            var shareUrl = $"{Request.Scheme}://{Request.Host}/Event/Details/{id}";
            var shareText = $"Check out this event: {eventItem.Title} - {shareUrl}";

            // In a real app, this would integrate with social media APIs
            TempData["Success"] = $"Share link copied: {shareText}";
            return RedirectToAction("Details", new { id });
        }

        private List<Event> GetUpcomingEvents()
        {
            return new List<Event>
            {
                new() { Id = 1, Title = "Grant Writing Masterclass for African CSOs", Type = "Workshop", Format = "Virtual", Date = "2024-03-20", Time = "14:00 - 17:00 GMT", Duration = "3 hours", Instructor = "Dr. Amara Okafor", Participants = 45, MaxParticipants = 50, Price = "Free", Level = "Intermediate", Description = "Learn advanced grant writing techniques specifically tailored for African CSOs. Cover proposal structure, budget planning, and impact measurement.", Agenda = new() { "Understanding donor priorities in Africa", "Crafting compelling narratives", "Budget development and justification", "Impact measurement frameworks", "Q&A and feedback session" }, Materials = new() { "Workbook PDF", "Template Library", "Recording Access" }, Tags = new() { "Grant Writing", "Fundraising", "Capacity Building" } },
                new() { Id = 2, Title = "CSO Leadership Summit 2025", Type = "Conference", Format = "Hybrid", Date = "2024-04-15", Time = "09:00 - 17:00 GMT", Duration = "2 days", Instructor = "Multiple Speakers", Participants = 120, MaxParticipants = 200, Price = "R75", Level = "All Levels", Location = "Johannesburg, South Africa", Description = "Annual summit bringing together CSO leaders from across Africa to share insights, network, and learn about emerging trends.", Agenda = new() { "Opening keynote: Future of Civil Society", "Panel: Digital Transformation for CSOs", "Workshop: Sustainable Financing Models", "Networking lunch", "Case studies: Successful CSO innovations", "Closing ceremony and awards" }, Materials = new() { "Conference Kit", "Networking Directory", "Session Recordings" }, Tags = new() { "Leadership", "Networking", "Innovation" } },
                new() { Id = 3, Title = "Financial Management for Small CSOs", Type = "Webinar", Format = "Virtual", Date = "2024-03-25", Time = "15:00 - 16:30 GMT", Duration = "1.5 hours", Instructor = "Sarah Mensah", Participants = 28, MaxParticipants = 100, Price = "Free", Level = "Beginner", Description = "Essential financial management practices for small and emerging CSOs, including bookkeeping, reporting, and compliance.", Agenda = new() { "Basic financial management principles", "Setting up simple accounting systems", "Donor reporting requirements", "Cash flow management", "Live Q&A session" }, Materials = new() { "Financial Templates", "Checklist", "Resource Links" }, Tags = new() { "Finance", "Compliance", "Management" } },
                new() { Id = 4, Title = "Digital Advocacy and Social Media Strategies", Type = "Workshop", Format = "Virtual", Date = "2024-04-05", Time = "10:00 - 13:00 GMT", Duration = "3 hours", Instructor = "Rachel Mwangi", Participants = 32, MaxParticipants = 75, Price = "Free", Level = "Beginner", Description = "Master digital advocacy techniques and social media strategies to amplify your CSO's voice and impact.", Agenda = new() { "Digital advocacy fundamentals", "Social media strategy development", "Content creation for impact", "Engagement and community building", "Measuring advocacy success" }, Materials = new() { "Strategy Templates", "Content Calendar", "Analytics Guide" }, Tags = new() { "Digital Advocacy", "Social Media", "Communication" } },
                new() { Id = 5, Title = "Cyber Security Gartner IT Symposium", Type = "Training", Format = "In-Person", Date = "2024-04-22", Time = "09:00 - 16:00 GMT", Duration = "7 hours", Instructor = "Dr. Fatima Ndour", Participants = 18, MaxParticipants = 30, Price = "R150", Level = "Intermediate", Location = "Nairobi, Kenya", Description = "Comprehensive training on monitoring and evaluation frameworks specifically designed for civil society organizations.", Agenda = new() { "M&E fundamentals for CSOs", "Developing logical frameworks", "Data collection methods", "Impact measurement techniques", "Reporting and learning", "Practical exercises" }, Materials = new() { "M&E Toolkit", "Case Studies", "Templates", "Resource Guide" }, Tags = new() { "Monitoring", "Evaluation", "Impact Assessment" } },
                new() { Id = 6, Title = "Sustainable Fundraising Models", Type = "Webinar", Format = "Virtual", Date = "2024-05-10", Time = "14:00 - 15:30 GMT", Duration = "1.5 hours", Instructor = "Ahmed Ali", Participants = 67, MaxParticipants = 150, Price = "Free", Level = "All Levels", Description = "Explore diverse and sustainable fundraising models that can ensure long-term financial stability for your CSO.", Agenda = new() { "Traditional vs. innovative fundraising", "Membership and subscription models", "Social enterprise approaches", "Partnership and collaboration strategies", "Digital fundraising trends" }, Materials = new() { "Fundraising Models Guide", "Case Study Collection", "Implementation Checklist" }, Tags = new() { "Fundraising", "Sustainability", "Financial Planning" } }
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