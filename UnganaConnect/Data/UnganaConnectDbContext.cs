using Microsoft.EntityFrameworkCore;
using UnganaConnect.Frontend.Models;
using UnganaConnect.Models.Blog;
using UnganaConnect.Models.Course;
using UnganaConnect.Models.User;

namespace UnganaConnect.Data
{
    public class UnganaConnectDbContext : DbContext
    {
        public UnganaConnectDbContext(DbContextOptions<UnganaConnectDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseEnrollment> Enrollments { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizOption> QuizOptions { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<ForumCategory> ForumCategories { get; set; }
        public DbSet<ForumTopic> ForumTopics { get; set; }
        public DbSet<ForumReply> ForumReplies { get; set; }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }
        
        public DbSet<BlogPost> BlogPosts { get; set; }

    }
}
