namespace UnganaConnect.Models.Users
{
    public class Admin
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public string ActionType { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
