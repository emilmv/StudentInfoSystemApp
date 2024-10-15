using StudentInfoSystemApp.Application.Helpers.MapProfiles;
namespace StudentInfoSystemApp.Presentation.Extensions
{
    public static class AutomapperExtensions
    {
        public static IServiceCollection AddCustomAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AttendanceMapProfile).Assembly);
            services.AddAutoMapper(opt =>
            {
                opt.AddProfile(new StudentMapProfile(new HttpContextAccessor()));
                opt.AddProfile(new EnrollmentMapProfile(new HttpContextAccessor()));
                opt.AddProfile(new InstructorMapProfile(new HttpContextAccessor()));
            });
            return services;
        }
    }
}
