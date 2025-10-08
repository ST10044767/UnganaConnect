using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.Frontend.Models
{
    public class ConsultancyRequestViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Area { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Consultant { get; set; } = string.Empty;
        public DateTime Submitted { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public string BudgetRange { get; set; } = string.Empty;
    }

    public class CreateConsultancyRequestViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Area { get; set; } = string.Empty;
        [Required]
        public string Priority { get; set; } = string.Empty;
        [Required]
        public DateTime Deadline { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        public string BudgetRange { get; set; } = string.Empty;
    }

    public class ConsultantViewModel
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Expertise { get; set; } = new();
        public string Experience { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<string> Languages { get; set; } = new();
    }

    public class ConsultancyIndexViewModel
    {
        public List<ConsultancyRequestViewModel> MyRequests { get; set; } = new();
        public List<ConsultantViewModel> AvailableConsultants { get; set; } = new();
        public List<string> ConsultancyAreas { get; set; } = new();
    }
}