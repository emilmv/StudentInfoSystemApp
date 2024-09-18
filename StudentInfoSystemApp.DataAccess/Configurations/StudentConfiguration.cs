using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.DataAccess.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(s => s.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.DateOfBirth)
                .IsRequired();

            builder.Property(s => s.Gender)
                .HasMaxLength(10);

            builder.Property(s => s.Email)
                .HasMaxLength(255);

            builder.Property(s => s.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(s => s.Address)
                .HasMaxLength(500);

            builder.Property(s => s.EnrollmentDate)
                .IsRequired();

            builder.Property(s => s.Status)
                .HasDefaultValue("Active")
                .HasMaxLength(20);

            builder.Property(s=>s.Photo)
                .IsRequired();

            builder.HasOne(s => s.Program)
                .WithMany(p => p.Students)
                .HasForeignKey(s => s.ProgramID);
                //.OnDelete(DeleteBehavior.Cascade);
        }
    }
}
