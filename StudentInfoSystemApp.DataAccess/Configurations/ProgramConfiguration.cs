using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.DataAccess.Configurations
{
    public class ProgramConfiguration : IEntityTypeConfiguration<Program>
    {
        public void Configure(EntityTypeBuilder<Program> builder)
        {
            builder.Property(p => p.ProgramName)
            .IsRequired()
            .HasMaxLength(255);

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            builder.Property(p => p.RequiredCredits)
                .IsRequired();

            builder.HasMany(p => p.Students)
                .WithOne(s => s.Program)
                .HasForeignKey(s => s.ProgramID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Courses)
                .WithOne(c => c.Program)
                .HasForeignKey(c => c.ProgramID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
