using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.Frontend.Models
{
    public class ImpactMetricViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Change { get; set; } = string.Empty;
        public string ChangeType { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class ProgramProgressViewModel
    {
        public string Name { get; set; } = string.Empty;
        public int Progress { get; set; }
        public string Target { get; set; } = string.Empty;
        public string Achieved { get; set; } = string.Empty;
        public string Budget { get; set; } = string.Empty;
        public string Spent { get; set; } = string.Empty;
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class SDGAlignmentViewModel
    {
        public string Goal { get; set; } = string.Empty;
        public int Alignment { get; set; }
        public int Programs { get; set; }
    }

    public class DonorReportViewModel
    {
        public string Donor { get; set; } = string.Empty;
        public string Project { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class ImpactDashboardViewModel
    {
        public List<ImpactMetricViewModel> ImpactMetrics { get; set; } = new();
        public List<ProgramProgressViewModel> ProgramProgress { get; set; } = new();
        public List<SDGAlignmentViewModel> SDGAlignment { get; set; } = new();
        public List<DonorReportViewModel> DonorReports { get; set; } = new();
        public string SelectedTimeframe { get; set; } = "6months";
    }
}