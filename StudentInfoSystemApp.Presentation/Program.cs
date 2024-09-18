using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.Implementations;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.Application.MapProfiles;
using StudentInfoSystemApp.DataAccess.Data;

var builder = WebApplication.CreateBuilder(args);
var configuration= builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StudentInfoSystemContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IAttendanceService,AttendanceService>();
builder.Services.AddScoped<ICourseService,CourseService>();
builder.Services.AddScoped<IDepartmentService,DepartmentService>();
builder.Services.AddScoped<IEnrollmentService,EnrollmentService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(opt => 
opt.AddProfile(new EnrollmentMapProfile(new HttpContextAccessor())));
builder.Services.AddAutoMapper(typeof(AttendanceMapProfile).Assembly);

var app = builder.Build();

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
