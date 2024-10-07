using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Core.Entities;
using System.Reflection;

namespace StudentInfoSystemApp.DataAccess.Data
{
    public class StudentInfoSystemContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Student> Students { get; set; }

        public StudentInfoSystemContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Seed Roles
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Owner",
                    NormalizedName = "OWNER"
                },
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                }
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);

            // Seed Users
            var hasher = new PasswordHasher<ApplicationUser>();
            List<ApplicationUser> users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "owner",
                    NormalizedUserName = "OWNER",
                    Email = "owner@example.com",
                    NormalizedEmail = "OWNER@EXAMPLE.COM",
                    PasswordHash = hasher.HashPassword(null, "DefaultPassword123!"),
                    EmailConfirmed = true
                }
            };
            modelBuilder.Entity<ApplicationUser>().HasData(users);

            // Seed UserRoles
            List<IdentityUserRole<string>> userRoles = new()
            {
                new IdentityUserRole<string>
                {
                    RoleId = roles.First(r => r.Name == "Owner").Id,
                    UserId = users.First(u => u.UserName == "owner").Id
                }
            };
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(userRoles);

            base.OnModelCreating(modelBuilder);
        }
    }
}
