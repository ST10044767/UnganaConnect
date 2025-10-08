using System.ComponentModel.DataAnnotations;

namespace UnganaConnect.Frontend.Models
{
    public class SettingsViewModel
    {
        public UserProfileModel UserProfile { get; set; } = new();
        public OrganizationProfileModel OrganizationProfile { get; set; } = new();
        public NotificationSettingsModel NotificationSettings { get; set; } = new();
        public SecuritySettingsModel SecuritySettings { get; set; } = new();
        public List<ActivityModel> RecentActivity { get; set; } = new();
        public List<AchievementModel> Achievements { get; set; } = new();
        public ProfileStatsModel ProfileStats { get; set; } = new();
    }

    public class UserProfileModel
    {
        [Required]
        public string Name { get; set; } = "";
        [Required, EmailAddress]
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Role { get; set; } = "";
        public string Organization { get; set; } = "";
        public string Location { get; set; } = "";
        public string Website { get; set; } = "";
        public string Bio { get; set; } = "";
        public DateTime JoinDate { get; set; }
        public List<string> Languages { get; set; } = new();
        public List<string> Expertise { get; set; } = new();
        public string Avatar { get; set; } = "";
    }

    public class OrganizationProfileModel
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string RegistrationNumber { get; set; } = "";
        public string TaxId { get; set; } = "";
        public string FoundedYear { get; set; } = "";
        public string Website { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> FocusAreas { get; set; } = new();
        public int Staff { get; set; }
        public int Volunteers { get; set; }
        public int Beneficiaries { get; set; }
    }

    public class NotificationSettingsModel
    {
        public bool EmailNotifications { get; set; }
        public bool CourseUpdates { get; set; }
        public bool EventReminders { get; set; }
        public bool ForumActivity { get; set; }
        public bool WeeklyDigest { get; set; }
        public bool MarketingEmails { get; set; }
        public bool SmsNotifications { get; set; }
        public bool PushNotifications { get; set; }
    }

    public class SecuritySettingsModel
    {
        public bool TwoFactorAuth { get; set; }
        public bool LoginNotifications { get; set; }
        public bool DataExport { get; set; }
        public string ProfileVisibility { get; set; } = "public";
    }

    public class ActivityModel
    {
        public string Action { get; set; } = "";
        public string Item { get; set; } = "";
        public DateTime Date { get; set; }
        public string Type { get; set; } = "";
    }

    public class AchievementModel
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime EarnedDate { get; set; }
        public string Icon { get; set; } = "";
    }

    public class ProfileStatsModel
    {
        public int CompletionPercentage { get; set; }
        public int CoursesCompleted { get; set; }
        public int ForumPosts { get; set; }
        public int ResourcesDownloaded { get; set; }
    }

    public class PasswordChangeModel
    {
        [Required]
        public string CurrentPassword { get; set; } = "";
        [Required, MinLength(6)]
        public string NewPassword { get; set; } = "";
        [Required, Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = "";
    }
}