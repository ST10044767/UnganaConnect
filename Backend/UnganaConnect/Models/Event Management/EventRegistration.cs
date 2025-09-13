namespace UnganaConnect.Models.Event_Management
{

    public class EventRegistration
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public DateTime RegisteredAt { get; set; }
    }

}
