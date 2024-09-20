using StudentInfoSystemApp.Application.Exceptions;

namespace StudentInfoSystemApp.Presentation.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                var errors = new Dictionary<string, string>();
                context.Response.StatusCode = 500;
                if (ex is CustomException customException)
                {
                    message ="Something went wrong. Please check errors.";
                    errors = customException.Errors;
                    context.Response.StatusCode = customException.StatusCode;
                }
                await context.Response.WriteAsJsonAsync(new { message, errors });
            }
        }
    }
}

