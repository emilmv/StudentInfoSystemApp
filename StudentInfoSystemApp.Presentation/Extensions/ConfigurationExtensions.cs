using StudentInfoSystemApp.Application.Settings;

namespace StudentInfoSystemApp.Presentation.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection addCustomSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.Configure<DbSettings>(configuration.GetSection("ConnectionStrings"));

            return services;
        }
    }
}
