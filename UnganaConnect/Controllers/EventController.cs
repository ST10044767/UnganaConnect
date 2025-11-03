using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Frontend.Models;

namespace UnganaConnect.Frontend.Controllers
{
    public class EventController : Controller
    {
        private readonly UnganaConnectDbContext _context;

        public EventController(UnganaConnectDbContext context)
        {
            _context = context;
        }

        // ==========================
        // List upcoming events and user's events
        // ==========================
        public async Task<IActionResult> Index()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            var upcomingEvents = await _context.Events
                .Include(e => e.Registrations)
                .OrderBy(e => e.Date)
                .ToListAsync();

            List<MyEventViewModel> myEvents = new();

            if (!string.IsNullOrEmpty(userEmail))
            {
                myEvents = await _context.EventRegistrations
                    .Where(r => r.UserEmail == userEmail)
                    .Include(r => r.Event)
                    .Select(r => new MyEventViewModel
                    {
                        EventId = r.EventId,
                        Title = r.Event.Title,
                        StartDateTime = r.Event.Date,
                        Status = r.Status,
                        Type = r.Event.Type
                    })
                    .ToListAsync();
            }

            var viewModel = new EventViewModel
            {
                UpcomingEvents = upcomingEvents,
                MyEvents = myEvents
            };

            return View(viewModel);
        }

        // ==========================
        // Event details
        // ==========================
        public async Task<IActionResult> Details(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null)
                return NotFound();

            var userEmail = HttpContext.Session.GetString("UserEmail");
            bool isRegistered = false;
            string? registrationStatus = null;

            if (!string.IsNullOrEmpty(userEmail))
            {
                var registration = await _context.EventRegistrations
                    .FirstOrDefaultAsync(r => r.EventId == id && r.UserEmail == userEmail);

                isRegistered = registration != null;
                registrationStatus = registration?.Status;
            }

            ViewBag.IsRegistered = isRegistered;
            ViewBag.RegistrationStatus = registrationStatus;
            ViewBag.IsEventFull = eventItem.Registrations.Count >= eventItem.MaxParticipants;
            ViewBag.CurrentRegistrations = eventItem.Registrations.Count;

            return View(eventItem);
        }

        // ==========================
        // GET Register
        // ==========================
        [HttpGet]
        public async Task<IActionResult> Register(int eventId)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Error"] = "Please log in to register for events.";
                return RedirectToAction("Login", "Auth");
            }

            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                TempData["Error"] = "Event not found.";
                return RedirectToAction("Index");
            }

            // Already registered?
            var existingRegistration = await _context.EventRegistrations
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserEmail == userEmail);

            if (existingRegistration != null)
            {
                TempData["Info"] = $"You are already registered for this event (Status: {existingRegistration.Status}).";
                return RedirectToAction("Details", new { id = eventId });
            }

            var model = new EventRegistrationViewModel
            {
                EventId = eventId,
                EventTitle = eventItem.Title,
                EventDateTime = eventItem.Date,
                EventLocation = eventItem.Location,
                Email = userEmail
            };

            return View(model);
        }

        // ==========================
        // POST Register
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(EventRegistrationViewModel model)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Error"] = "Please log in to register for events.";
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields and accept the terms and conditions.";
                return View(model);
            }

            var eventItem = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == model.EventId);

            if (eventItem == null)
            {
                TempData["Error"] = "Event not found.";
                return RedirectToAction("Index");
            }

            if (eventItem.Registrations.Count >= eventItem.MaxParticipants)
            {
                TempData["Error"] = "Event is full.";
                return RedirectToAction("Details", new { id = model.EventId });
            }

            var registration = new EventRegistration
            {
                EventId = model.EventId,
                UserEmail = userEmail,
                RegistrationDate = DateTime.UtcNow,
                Status = "Registered"
            };

            _context.EventRegistrations.Add(registration);
            await _context.SaveChangesAsync();

            // Optional: Update participant count dynamically after saving
            eventItem.Participants = await _context.EventRegistrations.CountAsync(r => r.EventId == eventItem.Id);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Successfully registered for event!";
            return RedirectToAction("Details", new { id = model.EventId });
        }

        // ==========================
        // Admin/Instructor: CRUD
        // ==========================
        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction("Index");
            }

            return View(new Event());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event model, string? TimeInput)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction("Index");
            }


            if(!string.IsNullOrEmpty(TimeInput) && TimeSpan.TryParse(TimeInput, out var timeSpan))
    model.Time = timeSpan;

            // Ensure UTC kind
            model.Date = DateTime.SpecifyKind(model.Date, DateTimeKind.Utc);

            if (!ModelState.IsValid)
                return View(model);

            model.Participants = 0;
            _context.Events.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Event created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction("Index");
            }

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null) return NotFound();

            return View(eventItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Event model, string? TimeInput)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(TimeInput) && TimeSpan.TryParse(TimeInput, out var timeSpan))
                model.Time = timeSpan;

            if (!ModelState.IsValid)
                return View(model);

            var eventItem = await _context.Events.FindAsync(model.Id);
            if (eventItem == null)
                return NotFound();

            eventItem.Title = model.Title;
            eventItem.Type = model.Type;
            eventItem.Format = model.Format;
            eventItem.Date = model.Date;
            eventItem.Time = model.Time;
            eventItem.Duration = model.Duration;
            eventItem.Price = model.Price;
            eventItem.Instructor = model.Instructor;
            eventItem.MaxParticipants = model.MaxParticipants;
            eventItem.Level = model.Level;
            eventItem.Location = model.Location;
            eventItem.Description = model.Description;
            eventItem.Agenda = model.Agenda;
            eventItem.Materials = model.Materials;
            eventItem.Tags = model.Tags;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Event updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction("Index");
            }

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
                return NotFound();

            return View(eventItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction("Index");
            }

            var eventItem = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem != null)
            {
                _context.EventRegistrations.RemoveRange(eventItem.Registrations);
                _context.Events.Remove(eventItem);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Event deleted successfully!";
            }

            return RedirectToAction("Index");
        }
    }
}
