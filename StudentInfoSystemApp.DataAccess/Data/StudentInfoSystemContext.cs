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

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Restrict;

            // Seed Roles
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole{Name = "Owner",NormalizedName = "OWNER"},
                new IdentityRole{Name = "Admin",NormalizedName = "ADMIN"},
                new IdentityRole{Name = "User",NormalizedName = "USER"}
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
                    FullName ="ExampleName ExampleSurname",
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
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }
        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreateDate = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdateDate = DateTime.Now;
                }
            }
        }
    }
}
