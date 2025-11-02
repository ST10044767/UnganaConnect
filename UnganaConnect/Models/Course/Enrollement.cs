using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace UnganaConnect.Models.Course
{
    public class CourseEnrollment
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public int CourseId { get; set; }
        [ValidateNever]
        public Course? Course { get; set; }

        public double Progress { get; set; } = 0;
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public bool Completed { get; set; } = false;
    }



}
