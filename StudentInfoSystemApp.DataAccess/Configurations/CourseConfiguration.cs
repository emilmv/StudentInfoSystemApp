using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.DataAccess.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.Property(c => c.CourseName)
            .IsRequired()
            .HasMaxLength(255);

            builder.Property(c => c.CourseCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Description)
                .HasMaxLength(1000);

            builder.Property(c => c.Credits)
                .IsRequired();

            builder.HasOne(c => c.Program)
                .WithMany(p => p.Courses)
                .HasForeignKey(c => c.ProgramID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Enrollments)
                .WithOne(c => c.Course)
                .HasForeignKey(c => c.CourseID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Schedules)
                .WithOne(cs => cs.Course)
                .HasForeignKey(cs => cs.CourseID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
