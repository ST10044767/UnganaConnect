using Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UnganaConnect.Controllers;
using UnganaConnect.Models;
using UnganaConnect.Models.Consultancy_Management;
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


        // Training & Learning
        public DbSet<Course> Courses { get; set; }

        public DbSet<Module> Modules { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Certificate> Certificates { get; set; }

        // Resource Repository
        public DbSet<CResource> Resources { get; set; }
        public DbSet<ResourceEngagement> ResourceEngagements { get; set; }


        // Event Management
        public DbSet<Event> Events { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }

        // Community Forum
        public DbSet<ForumThread> ForumThreads { get; set; }
        public DbSet<ForumReply> ForumReplies { get; set; }

        // Consultancy Management
        public DbSet<ConsultancyRequest> ConsultancyRequests { get; set; }

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ForumThread configuration
        modelBuilder.Entity<ForumThread>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ForumReply configuration
        modelBuilder.Entity<ForumReply>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.RepliedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Thread)
                .WithMany(t => t.Replies)
                .HasForeignKey(e => e.ThreadId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
