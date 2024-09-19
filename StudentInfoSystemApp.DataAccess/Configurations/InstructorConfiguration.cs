using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.DataAccess.Configurations
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.Property(i => i.FirstName)
             .IsRequired()
             .HasMaxLength(100);

            builder.Property(i => i.Photo)
                .IsRequired();

            builder.Property(i => i.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.Email)
                .HasMaxLength(255);

            builder.Property(i => i.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(i => i.HireDate)
                .IsRequired()
                .HasDefaultValueSql("GetDate()");

            builder.HasOne(i => i.Department)
                .WithMany(d => d.Instructors)
                .HasForeignKey(i => i.DepartmentID);
            //.OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.Schedules)
                .WithOne(cs => cs.Instructor)
                .HasForeignKey(cs => cs.InstructorID);
               // .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
