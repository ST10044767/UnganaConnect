using Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UnganaConnect.Controllers;
using UnganaConnect.Models;
using UnganaConnect.Models.Event_Management;
using UnganaConnect.Models.Forum;
using UnganaConnect.Models.Resources_Repo;
using UnganaConnect.Models.Training___Learning;
using UnganaConnect.Models.Users;

namespace UnganaConnect.Data
{
    public class UnganaConnectDbcontext : DbContext
    {
        public UnganaConnectDbcontext(DbContextOptions<UnganaConnectDbcontext> options)
            : base(options)
        {
        }

        // Authentication & Administration
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> AdminActions { get; set; }


        // Training & Learning
        public DbSet<Course> Courses { get; set; }

        public DbSet<Module> Modules { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<UserProgress> UserProgress { get; set; }

        // Resource Repository
        public DbSet<CResource> Resources { get; set; }
        public DbSet<ResourceEngagement> ResourceEngagements { get; set; }


        // Event Management
        public DbSet<Event> Events { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }

        // Community Forum
        public DbSet<ForumThread> ForumThreads { get; set; }
        public DbSet<ForumReply> ForumReplies { get; set; }

    }
}
