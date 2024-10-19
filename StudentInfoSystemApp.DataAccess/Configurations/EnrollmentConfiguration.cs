using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.DataAccess.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.Property(e => e.EnrollmentDate)
             .IsRequired()
             .HasDefaultValueSql("GetDate()");

            builder.Property(e => e.Grade)
                .HasMaxLength(5);

            builder.Property(e => e.Semester)
                .HasMaxLength(50);

            builder.HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Attendances)
                .WithOne(a => a.Enrollment)
                .HasForeignKey(a => a.EnrollmentID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
