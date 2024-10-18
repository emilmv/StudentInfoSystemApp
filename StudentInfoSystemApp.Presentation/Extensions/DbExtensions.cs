using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.Services.Implementations;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Presentation.Extensions
{
    public static class DbExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<StudentInfoSystemContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IInstructorService, InstructorService>();
            services.AddScoped<IProgramService, ProgramService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped(typeof(IPaginationService<>), typeof(PaginationService<>));

            return services;
        }
    }
}
