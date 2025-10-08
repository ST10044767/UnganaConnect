using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Frontend.Models;
using UnganaConnect.Frontend.Services;

namespace UnganaConnect.Frontend.Controllers
{
    public class ImpactController : Controller
    {
        private readonly ApiService _apiService;

        public ImpactController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index(string timeframe = "6months")
        {
            var model = new ImpactDashboardViewModel
            {
                ImpactMetrics = GetImpactMetrics(),
                ProgramProgress = GetProgramProgress(),
                SDGAlignment = GetSDGAlignment(),
                DonorReports = GetDonorReports(),
                SelectedTimeframe = timeframe
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExportReport()
        {
            var token = HttpContext.Session.GetString("Token");
            var response = await _apiService.PostAsync("impact/export", new { }, token);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Report exported successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to export report.";
            }

            return RedirectToAction("Index");
        }

        private List<ImpactMetricViewModel> GetImpactMetrics()
        {
            return new List<ImpactMetricViewModel>
            {
                new() { Title = "Lives Impacted", Value = "12,450", Change = "+15.3%", ChangeType = "increase", Icon = "fas fa-heart", Color = "text-danger", Description = "Direct beneficiaries reached through programs" },
                new() { Title = "Communities Served", Value = "45", Change = "+8", ChangeType = "increase", Icon = "fas fa-globe", Color = "text-primary", Description = "Geographic communities with active programs" },
                new() { Title = "Programs Completed", Value = "23", Change = "+12%", ChangeType = "increase", Icon = "fas fa-bullseye", Color = "text-success", Description = "Successfully completed program cycles" },
                new() { Title = "Funding Secured", Value = "R2.3M", Change = "+28%", ChangeType = "increase", Icon = "fas fa-dollar-sign", Color = "text-purple", Description = "Total funding raised this period" }
            };
        }

        private List<ProgramProgressViewModel> GetProgramProgress()
        {
            return new List<ProgramProgressViewModel>
            {
                new() { Name = "Education Access Initiative", Progress = 85, Target = "5,000 students", Achieved = "4,250 students", Budget = "R450,000", Spent = "R382,500", EndDate = DateTime.Parse("2024-12-31"), Status = "On Track" },
                new() { Name = "Clean Water Project", Progress = 92, Target = "15 wells", Achieved = "14 wells", Budget = "R200,000", Spent = "R184,000", EndDate = DateTime.Parse("2024-06-30"), Status = "Ahead of Schedule" },
                new() { Name = "Women Empowerment Program", Progress = 68, Target = "500 women", Achieved = "340 women", Budget = "R150,000", Spent = "R102,000", EndDate = DateTime.Parse("2024-09-30"), Status = "Needs Attention" },
                new() { Name = "Youth Skills Development", Progress = 45, Target = "200 participants", Achieved = "90 participants", Budget = "R75,000", Spent = "R33,750", EndDate = DateTime.Parse("2024-11-30"), Status = "Early Stage" }
            };
        }

        private List<SDGAlignmentViewModel> GetSDGAlignment()
        {
            return new List<SDGAlignmentViewModel>
            {
                new() { Goal = "SDG 1: No Poverty", Alignment = 85, Programs = 3 },
                new() { Goal = "SDG 3: Good Health", Alignment = 72, Programs = 2 },
                new() { Goal = "SDG 4: Quality Education", Alignment = 95, Programs = 4 },
                new() { Goal = "SDG 5: Gender Equality", Alignment = 68, Programs = 2 },
                new() { Goal = "SDG 6: Clean Water", Alignment = 88, Programs = 1 },
                new() { Goal = "SDG 8: Decent Work", Alignment = 60, Programs = 2 }
            };
        }

        private List<DonorReportViewModel> GetDonorReports()
        {
            return new List<DonorReportViewModel>
            {
                new() { Donor = "Gates Foundation", Project = "Education Access Initiative", DueDate = DateTime.Parse("2024-03-31"), Status = "Draft", Type = "Quarterly Report" },
                new() { Donor = "USAID", Project = "Clean Water Project", DueDate = DateTime.Parse("2024-04-15"), Status = "Submitted", Type = "Progress Report" },
                new() { Donor = "European Union", Project = "Women Empowerment Program", DueDate = DateTime.Parse("2024-04-30"), Status = "Pending", Type = "Financial Report" },
                new() { Donor = "Ford Foundation", Project = "Youth Skills Development", DueDate = DateTime.Parse("2024-05-15"), Status = "In Review", Type = "Impact Assessment" }
            };
        }
    }
}