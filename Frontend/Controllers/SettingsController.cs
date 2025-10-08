using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Frontend.Models;
using UnganaConnect.Frontend.Services;

namespace UnganaConnect.Frontend.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ApiService _apiService;

        public SettingsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            var model = GetSampleSettingsData();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfileModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Call API to update profile
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Index");
            }
            
            var settingsModel = GetSampleSettingsData();
            settingsModel.UserProfile = model;
            return View("Index", settingsModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrganization(OrganizationProfileModel model)
        {
            // TODO: Call API to update organization
            TempData["Success"] = "Organization details updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNotifications(NotificationSettingsModel model)
        {
            // TODO: Call API to update notifications
            TempData["Success"] = "Notification settings updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSecurity(SecuritySettingsModel model)
        {
            // TODO: Call API to update security settings
            TempData["Success"] = "Security settings updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(PasswordChangeModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Call API to change password
                TempData["Success"] = "Password changed successfully!";
                return RedirectToAction("Index");
            }
            
            TempData["Error"] = "Please check your password details.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ExportData()
        {
            // TODO: Generate and return user data export
            TempData["Success"] = "Data export will be sent to your email.";
            return RedirectToAction("Index");
        }

        private SettingsViewModel GetSampleSettingsData()
        {
            return new SettingsViewModel
            {
                UserProfile = new UserProfileModel
                {
                    Name = "Sarah Mensah",
                    Email = "sarah.mensah@hopeinitiative.org",
                    Phone = "+233 24 567 8901",
                    Role = "Program Director",
                    Organization = "Hope Initiative Ghana",
                    Location = "Accra, Ghana",
                    Website = "https://hopeinitiative.org",
                    Bio = "Experienced program director with 8+ years in CSO management, specializing in education and community development initiatives across West Africa.",
                    JoinDate = new DateTime(2022, 3, 15),
                    Languages = new List<string> { "English", "Twi", "French" },
                    Expertise = new List<string> { "Project Management", "Grant Writing", "Community Engagement", "Financial Management" }
                },
                OrganizationProfile = new OrganizationProfileModel
                {
                    Name = "Hope Initiative Ghana",
                    Type = "Non-Profit Organization",
                    RegistrationNumber = "CSO/2018/0432",
                    TaxId = "C0023456789",
                    FoundedYear = "2018",
                    Website = "https://hopeinitiative.org",
                    Address = "123 Independence Avenue, Accra, Ghana",
                    Phone = "+233 30 267 8901",
                    Email = "info@hopeinitiative.org",
                    Description = "Hope Initiative Ghana is dedicated to improving lives in underserved communities through sustainable education, healthcare, and empowerment programs.",
                    FocusAreas = new List<string> { "Education", "Healthcare", "Water & Sanitation", "Women Empowerment" },
                    Staff = 25,
                    Volunteers = 120,
                    Beneficiaries = 5000
                },
                NotificationSettings = new NotificationSettingsModel
                {
                    EmailNotifications = true,
                    CourseUpdates = true,
                    EventReminders = true,
                    ForumActivity = false,
                    WeeklyDigest = true,
                    MarketingEmails = false,
                    SmsNotifications = false,
                    PushNotifications = true
                },
                SecuritySettings = new SecuritySettingsModel
                {
                    TwoFactorAuth = false,
                    LoginNotifications = true,
                    DataExport = true,
                    ProfileVisibility = "public"
                },
                ProfileStats = new ProfileStatsModel
                {
                    CompletionPercentage = 85,
                    CoursesCompleted = 12,
                    ForumPosts = 34,
                    ResourcesDownloaded = 67
                },
                Achievements = new List<AchievementModel>
                {
                    new AchievementModel { Title = "Learning Champion", Description = "Completed 10+ courses", EarnedDate = new DateTime(2024, 2, 15), Icon = "🎓" },
                    new AchievementModel { Title = "Community Contributor", Description = "Active forum participant", EarnedDate = new DateTime(2024, 1, 20), Icon = "🤝" },
                    new AchievementModel { Title = "Resource Explorer", Description = "Downloaded 50+ resources", EarnedDate = new DateTime(2024, 1, 10), Icon = "📚" }
                },
                RecentActivity = new List<ActivityModel>
                {
                    new ActivityModel { Action = "Completed course", Item = "Grant Writing Fundamentals", Date = new DateTime(2024, 3, 10), Type = "learning" },
                    new ActivityModel { Action = "Posted in forum", Item = "Best practices for donor stewardship", Date = new DateTime(2024, 3, 8), Type = "community" },
                    new ActivityModel { Action = "Downloaded resource", Item = "Financial Management Toolkit", Date = new DateTime(2024, 3, 5), Type = "resource" },
                    new ActivityModel { Action = "Attended event", Item = "CSO Leadership Workshop", Date = new DateTime(2024, 3, 1), Type = "event" }
                }
            };
        }
    }
}