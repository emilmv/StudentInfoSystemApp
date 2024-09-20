using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.Implementations;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.Application.MapProfiles;
using StudentInfoSystemApp.DataAccess.Data;
using StudentInfoSystemApp.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers().ConfigureApiBehaviorOptions(opt =>
{
    opt.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Where(e => e.Value?.Errors.Count > 0)
        .Select(x => new Dictionary<string, string>() { { x.Key, x.Value.Errors.First().ErrorMessage } });
        return new BadRequestObjectResult(new { message = "Something went wrong, please check errors.", errors });
    };
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StudentInfoSystemContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IInstructorService, InstructorService>();
builder.Services.AddScoped<IProgramService, ProgramService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(AttendanceMapProfile).Assembly);

builder.Services.AddAutoMapper(opt =>
{
    opt.AddProfile(new StudentMapProfile(new HttpContextAccessor()));
    opt.AddProfile(new EnrollmentMapProfile(new HttpContextAccessor()));
    opt.AddProfile(new InstructorMapProfile(new HttpContextAccessor()));
});

var app = builder.Build();

// Custom exception handling middleware
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
