using Microsoft.AspNetCore.Mvc;
namespace StudentInfoSystemApp.Presentation.Extensions
{
    public static class ApiBehaviourExtensions
    {
        public static IServiceCollection AddCustomApiBehavior(this IServiceCollection services)
        {
            services.AddControllers().ConfigureApiBehaviorOptions(opt =>
            {
                opt.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .Select(x => new Dictionary<string, string> { { x.Key, x.Value.Errors.First().ErrorMessage } });

                    return new BadRequestObjectResult(new
                    {
                        message = "Something went wrong, please check errors.",
                        errors
                    });
                };
            });
            return services;
        }
    }
}
