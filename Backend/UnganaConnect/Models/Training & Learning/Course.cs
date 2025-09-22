namespace UnganaConnect.Models.Training___Learning
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }


        public List<Module> Modules { get; set; } = new();
    }


}
