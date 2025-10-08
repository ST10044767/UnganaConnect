namespace UnganaConnect.Frontend.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public int Participants { get; set; }
        public int MaxParticipants { get; set; }
        public string Price { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Agenda { get; set; } = new();
        public List<string> Materials { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }

    public class MyEvent
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double? Rating { get; set; }
    }

    public class EventViewModel
    {
        public List<Event> UpcomingEvents { get; set; } = new();
        public List<MyEvent> MyEvents { get; set; } = new();
    }
}