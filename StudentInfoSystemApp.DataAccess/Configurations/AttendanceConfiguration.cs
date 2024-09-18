using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.DataAccess.Configurations
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.Property(a => a.AttendanceDate)
                .IsRequired()
                .HasDefaultValueSql("GetDate()");

            builder.Property(a => a.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(a => a.Enrollment)
                .WithMany(ae => ae.Attendances)
                .HasForeignKey(a => a.EnrollmentID);
                //.OnDelete(DeleteBehavior.Cascade);
        }
    }
}
