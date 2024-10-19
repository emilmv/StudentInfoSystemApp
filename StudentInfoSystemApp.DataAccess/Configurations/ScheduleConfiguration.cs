using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.DataAccess.Configurations
{
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.Property(s => s.Semester)
            .HasMaxLength(50);

            builder.Property(s => s.ClassTime)
                .HasMaxLength(50);

            builder.Property(s => s.Classroom)
                .HasMaxLength(50);

            builder.HasOne(s => s.Course)
                .WithMany(c => c.Schedules)
                .HasForeignKey(s => s.CourseID)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Instructor)
                .WithMany(i => i.Schedules)
                .HasForeignKey(s => s.InstructorID)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
