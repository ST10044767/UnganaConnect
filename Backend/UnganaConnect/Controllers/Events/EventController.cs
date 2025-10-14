using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using UnganaConnect.Data;
using UnganaConnect.Models.Event_Management;

namespace UnganaConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly UnganaConnectDbcontext _context;

        public EventController(UnganaConnectDbcontext context)
        {
            _context = context;
        }

        // -----------------------------------------------------------
        // 🔹 GET ALL EVENTS (Public)
        // -----------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetEvents(
            [FromQuery] string? location = null,
            [FromQuery] DateTime? startDate = null)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(location))
                query = query.Where(e => e.Location.Contains(location));

            if (startDate.HasValue)
                query = query.Where(e => e.StartDate >= startDate.Value);

            var events = await query
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            return Ok(events);
        }

        // -----------------------------------------------------------
        // 🔹 GET SINGLE EVENT BY ID
        // -----------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null) return NotFound("Event not found.");

            return Ok(evt);
        }

        // -----------------------------------------------------------
        // 🔹 CREATE EVENT (Admin only)
        // -----------------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] Event evt)
        {
            _context.Events.Add(evt);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = evt.Id }, evt);
        }

        // -----------------------------------------------------------
        // 🔹 UPDATE EVENT (Admin only)
        // -----------------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event updated)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null) return NotFound("Event not found.");

            evt.Title = updated.Title;
            evt.Description = updated.Description;
            evt.StartDate = updated.StartDate;
            evt.EndDate = updated.EndDate;
            evt.Location = updated.Location;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // -----------------------------------------------------------
        // 🔹 DELETE EVENT (Admin only)
        // -----------------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null) return NotFound();

            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // -----------------------------------------------------------
        // 🔹 USER REGISTRATION FOR EVENT
        // -----------------------------------------------------------
        [Authorize]
        [HttpPost("{eventId}/register")]
        public async Task<IActionResult> RegisterForEvent(int eventId)
        {
            var evt = await _context.Events.FindAsync(eventId);
            if (evt == null) return NotFound("Event not found.");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var existing = await _context.EventRegistrations
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

            if (existing != null)
                return BadRequest("You are already registered for this event.");

            var registration = new EventRegistration
            {
                EventId = eventId,
                UserId = userId,
                RegisteredAt = DateTime.UtcNow
            };

            _context.EventRegistrations.Add(registration);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully registered for event." });
        }

        // -----------------------------------------------------------
        // 🔹 UNREGISTER FROM EVENT
        // -----------------------------------------------------------
        [Authorize]
        [HttpDelete("{eventId}/unregister")]
        public async Task<IActionResult> UnregisterFromEvent(int eventId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var registration = await _context.EventRegistrations
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

            if (registration == null)
                return NotFound("You are not registered for this event.");

            _context.EventRegistrations.Remove(registration);
            await _context.SaveChangesAsync();

            return Ok(new { message = "You have been unregistered from the event." });
        }

        // -----------------------------------------------------------
        // 🔹 GET USERS REGISTERED FOR EVENT (Admin only)
        // -----------------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpGet("{eventId}/registrations")]
        public async Task<IActionResult> GetEventRegistrations(int eventId)
        {
            var registrations = await _context.EventRegistrations
                .Include(r => r.Users)
                .Where(r => r.EventId == eventId)
                .ToListAsync();

            return Ok(registrations);
        }

        // -----------------------------------------------------------
        // 🔹 GET USER’S REGISTERED EVENTS
        // -----------------------------------------------------------
        [Authorize]
        [HttpGet("my-registrations")]
        public async Task<IActionResult> GetMyRegistrations()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var myEvents = await _context.EventRegistrations
                .Include(r => r.Events)
                .Where(r => r.UserId == userId)
                .Select(r => r.Events)
                .ToListAsync();

            return Ok(myEvents);
        }
    }
}
