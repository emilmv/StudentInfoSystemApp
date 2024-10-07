using StudentInfoSystemApp.Presentation.Extensions;
using StudentInfoSystemApp.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddCustomServices(configuration);
builder.Services.AddCustomValidation();
builder.Services.AddCustomSwagger();
builder.Services.AddCustomAutoMapper();
builder.Services.AddCustomApiBehavior();
builder.Services.AddCustomIdentity();
builder.Services.AddJWTAuthentication(configuration);
builder.Services.addCustomSettings(configuration);
builder.Services.AddHttpContextAccessor();

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();