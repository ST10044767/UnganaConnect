using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Frontend.Models;

namespace UnganaConnect.Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly UnganaConnectDbContext _context;

        public HomeController(UnganaConnectDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
   return View();   
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public async Task<IActionResult> Member()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth", new { area = "" });

            var guidUserId = Guid.Parse(userId);

            // Fetch all enrollments including related Course
            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.UserId == guidUserId)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();

            // Calculate stats
            var completedCourses = enrollments.Count(e => e.Completed);
            var resources = await _context.Resources.ToListAsync();
            var resourceDownloads = resources.Sum(r => r.Downloads);

            var now = DateTime.UtcNow;

            var events = await _context.Events
                .Where(e => e.Date >= now)
                .OrderBy(e => e.Date)
                .Take(2)
                .ToListAsync();


            var userRegistrations = await _context.EventRegistrations
                .Where(r => r.UserEmail == userEmail)
                .CountAsync();

            // Build ViewModel
            var model = new DashboardViewModel
            {
                RecentCourses = enrollments
                    .Select((e, index) => new RecentCourse
                    {
                        Id = index + 1,
                        CourseId = e.CourseId,
                        Title = e.Course.Title,
                        Progress = (int)Math.Round(e.Progress),
                        TimeLeft = e.Completed ? "Completed" : "In progress",
                        Status = e.Completed ? "Completed" : "In Progress",
                        Course = e.Course
                    })
                    .Take(3)
                    .ToList(),

                EnrolledCourses = enrollments
                    .Select(e => new EnrolledCourse
                    {
                        Title = e.Course.Title,
                        Description = e.Course.Description,
                        ThumbnailUrl = e.Course.ThumbnailUrl
                    })
                    .Take(12)
                    .ToList(),

                UpcomingEvents = events
                    .Select(e => new UpcomingEvent
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Date = e.Date,
                        Type = e.Type,
                        Format = e.Format,
                        Location = e.Location
                    })
                    .ToList(),

                Stats = new DashboardStats
                {
                    CoursesCompleted = completedCourses,
                    CertificatesEarned = completedCourses,
                    ResourcesDownloaded = resourceDownloads,
                    EventsAttended = userRegistrations
                }
            };

            return View(model);
        }

        public async Task<IActionResult> Admin()
        {
            await EnsureSampleData();

            var now = DateTime.UtcNow;
            

           
            // Calculate admin stats
            var totalCourses = await _context.Courses.CountAsync();
            var activeUsers = await _context.Users.CountAsync();
            var totalResources = await _context.Resources.CountAsync();
            var upcomingEvents = await _context.Events
                .Where(e => e.Date >= now)
                .CountAsync();
            var events = await _context.Events
                            .Where(e => e.Date >= now)
                            .OrderBy(e => e.Date)
                            .Take(2)
                            .ToListAsync();

            var model = new DashboardViewModel
            {
                UpcomingEvents = events
                    .Select(e => new UpcomingEvent
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Date = e.Date,
                        Type = e.Type,
                        Format = e.Format,
                        Location = e.Location
                    })
                    .ToList(),

                Stats = new DashboardStats
                {
                    TotalCourses = totalCourses,
                    ActiveUsers = activeUsers,
                    TotalResources = totalResources,
                    UpcomingEventsCount = upcomingEvents
                }
            };

            return View(model);
        }

        private async Task EnsureSampleData()
        {
            if (await _context.Courses.AnyAsync()) return;

            // Sample Courses
            var courses = new[]
            {
                new UnganaConnect.Models.Course.Course { Title = "Grant Writing Fundamentals", Description = "Learn the basics of writing successful grant proposals for NGOs and community organizations.", Category = "Fundraising", Level = "Beginner", Duration = "4 hours", Status = "available", CreatedAt = DateTime.UtcNow },
                new UnganaConnect.Models.Course.Course { Title = "Financial Management for CSOs", Description = "Master financial planning, budgeting, and reporting for civil society organizations.", Category = "Finance", Level = "Intermediate", Duration = "6 hours", Status = "available", CreatedAt = DateTime.UtcNow },
                new UnganaConnect.Models.Course.Course { Title = "Community Engagement Strategies", Description = "Effective methods for engaging and mobilizing communities for social change.", Category = "Community", Level = "Beginner", Duration = "3 hours", Status = "available", CreatedAt = DateTime.UtcNow },
                new UnganaConnect.Models.Course.Course { Title = "Digital Marketing for NGOs", Description = "Leverage digital platforms to amplify your organization's impact and reach.", Category = "Marketing", Level = "Intermediate", Duration = "5 hours", Status = "available", CreatedAt = DateTime.UtcNow },
                new UnganaConnect.Models.Course.Course { Title = "Project Management Essentials", Description = "Plan, execute, and monitor projects effectively in the development sector.", Category = "Management", Level = "Beginner", Duration = "4 hours", Status = "available", CreatedAt = DateTime.UtcNow },
                new UnganaConnect.Models.Course.Course { Title = "Impact Measurement & Evaluation", Description = "Learn to measure and evaluate the impact of your programs and initiatives.", Category = "Analytics", Level = "Advanced", Duration = "8 hours", Status = "available", CreatedAt = DateTime.UtcNow }
            };
            _context.Courses.AddRange(courses);

            // Sample Events
            var events = new[]
            {
                new UnganaConnect.Frontend.Models.Event { Title = "Annual CSO Leadership Summit", Type = "Conference", Format = "Hybrid", Date = DateTime.UtcNow.AddDays(30), Duration = "2 days", Instructor = "Dr. Sarah Mwangi", MaxParticipants = 200, Price = "Free", Level = "All Levels", Location = "Nairobi, Kenya", Description = "Join leaders from across Africa to discuss the future of civil society." },
                new UnganaConnect.Frontend.Models.Event { Title = "Grant Writing Workshop", Type = "Workshop", Format = "Virtual", Date = DateTime.UtcNow.AddDays(15), Duration = "4 hours", Instructor = "James Ochieng", MaxParticipants = 50, Price = "$25", Level = "Beginner", Description = "Hands-on workshop to improve your grant writing skills." },
                new UnganaConnect.Frontend.Models.Event { Title = "Financial Management Webinar", Type = "Webinar", Format = "Virtual", Date = DateTime.UtcNow.AddDays(7), Duration = "2 hours", Instructor = "Mary Kamau", MaxParticipants = 100, Price = "Free", Level = "Intermediate", Description = "Best practices in financial management for CSOs." },
                new UnganaConnect.Frontend.Models.Event { Title = "Community Mobilization Training", Type = "Training", Format = "In-Person", Date = DateTime.UtcNow.AddDays(45), Duration = "3 days", Instructor = "Peter Mbeki", MaxParticipants = 30, Price = "$50", Level = "All Levels", Location = "Lagos, Nigeria", Description = "Intensive training on community engagement and mobilization." },
                new UnganaConnect.Frontend.Models.Event { Title = "Digital Advocacy Seminar", Type = "Seminar", Format = "Hybrid", Date = DateTime.UtcNow.AddDays(20), Duration = "1 day", Instructor = "Grace Nyong", MaxParticipants = 75, Price = "$15", Level = "Intermediate", Description = "Learn to use digital tools for advocacy and campaigning." },
                new UnganaConnect.Frontend.Models.Event { Title = "Impact Measurement Conference", Type = "Conference", Format = "Virtual", Date = DateTime.UtcNow.AddDays(60), Duration = "2 days", Instructor = "Dr. Ahmed Hassan", MaxParticipants = 150, Price = "$30", Level = "Advanced", Description = "Latest trends and tools in impact measurement and evaluation." }
            };
            _context.Events.AddRange(events);

            // Sample Resources
            var resources = new[]
            {
                new Resource { Title = "Grant Proposal Template", Description = "Comprehensive template for writing effective grant proposals.", Category = "Templates", Type = "PDF", Downloads = 245, CreatedAt = DateTime.UtcNow },
                new Resource { Title = "Financial Planning Toolkit", Description = "Complete toolkit for financial planning and budgeting in CSOs.", Category = "Toolkits", Type = "ZIP", Downloads = 189, CreatedAt = DateTime.UtcNow },
                new Resource { Title = "Community Engagement Guide", Description = "Step-by-step guide for effective community engagement strategies.", Category = "Guides", Type = "PDF", Downloads = 312, CreatedAt = DateTime.UtcNow },
                new Resource { Title = "Social Media Strategy Template", Description = "Ready-to-use template for developing social media strategies.", Category = "Templates", Type = "DOCX", Downloads = 156, CreatedAt = DateTime.UtcNow },
                new Resource { Title = "Project Management Checklist", Description = "Comprehensive checklist for managing development projects.", Category = "Checklists", Type = "PDF", Downloads = 278, CreatedAt = DateTime.UtcNow },
                new Resource { Title = "Impact Measurement Framework", Description = "Framework for measuring and evaluating program impact.", Category = "Frameworks", Type = "PDF", Downloads = 203, CreatedAt = DateTime.UtcNow }
            };
            _context.Resources.AddRange(resources);

            // Sample Blog Posts
            var blogs = new[]
            {
                new UnganaConnect.Models.Blog.BlogPost { Title = "5 Essential Grant Writing Tips for African CSOs", Content = "Grant writing can be challenging, but these five essential tips will help African CSOs improve their success rate...", AuthorId = Guid.NewGuid(), Category = "Fundraising", Status = "published", CreatedAt = DateTime.UtcNow },
                new UnganaConnect.Models.Blog.BlogPost { Title = "Building Sustainable Partnerships in Development", Content = "Sustainable partnerships are the backbone of successful development work. Here's how to build and maintain them...", AuthorId = Guid.NewGuid(), Category = "Partnerships", Status = "published", CreatedAt = DateTime.UtcNow },
                new UnganaConnect.Models.Blog.BlogPost { Title = "Digital Transformation for Civil Society", Content = "The digital age presents both opportunities and challenges for civil society organizations...", AuthorId = Guid.NewGuid(), Category = "Technology", Status = "published", CreatedAt = DateTime.UtcNow },
                new UnganaConnect.Models.Blog.BlogPost { Title = "Community-Led Development: Lessons from the Field", Content = "Community-led development approaches have shown remarkable success across Africa...", AuthorId = Guid.NewGuid(), Category = "Community", Status = "published", CreatedAt = DateTime.UtcNow },
                new UnganaConnect.Models.Blog.BlogPost { Title = "Financial Transparency in CSOs: Best Practices", Content = "Financial transparency builds trust with donors and communities. Here are the best practices...", AuthorId = Guid.NewGuid(), Category = "Finance", Status = "published", CreatedAt = DateTime.UtcNow },
                new UnganaConnect.Models.Blog.BlogPost { Title = "Measuring Impact: Beyond Numbers", Content = "Impact measurement goes beyond simple metrics. Learn how to capture the full story...", AuthorId = Guid.NewGuid(), Category = "Impact", Status = "published", CreatedAt = DateTime.UtcNow }
            };
            _context.BlogPosts.AddRange(blogs);

            await _context.SaveChangesAsync();
        }
        
    }
}