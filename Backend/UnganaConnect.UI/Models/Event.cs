namespace UnganaConnect.UI.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; } = "";
        public List<EventRegistration> EventRegistrations { get; set; } = new();
    }

    public class EventRegistration
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public DateTime RegisteredAt { get; set; }
        public Event? Event { get; set; }
    }
}
