using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnganaConnect.Frontend.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;

        // Combined Date + Time
        public DateTime Date { get; set; } = DateTime.Now;
        public TimeSpan Time { get; set; } = TimeSpan.Zero;
        
        public string Duration { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;

        public string Instructor { get; set; } = string.Empty;
        public int Participants { get; set; }
        public int MaxParticipants { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // JSON-mapped lists
        public List<string> Agenda { get; set; } = new();
        public List<string> Materials { get; set; } = new();
        public List<string> Tags { get; set; } = new();

        // Navigation: registrations
        public List<EventRegistration> Registrations { get; set; } = new();
    }

    public class EventRegistration
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Event))]
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        [Required]
        public string UserEmail { get; set; } = string.Empty;

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string Status { get; set; } = "Registered"; // Registered, Completed, Cancelled
    }

    public class EventRegistrationViewModel
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public DateTime EventDateTime { get; set; }
        public string EventLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public string SpecialRequirements { get; set; } = string.Empty;

        [Required(ErrorMessage = "You must accept the terms and conditions.")]
        public bool AcceptTerms { get; set; }
    }

    public class EventViewModel
    {
        public List<Event> UpcomingEvents { get; set; } = new();
        public List<MyEventViewModel> MyEvents { get; set; } = new();
    }

    public class MyEventViewModel
    {
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
