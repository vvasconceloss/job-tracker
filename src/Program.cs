using JobTracker.Data;
using Scalar.AspNetCore;
using JobTracker.Middlewares;
using JobTracker.Services;
using JobTracker.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
  options.UseNpgsql(connectionString);
});

builder.Services.AddCors(o => o.AddPolicy("FrontendDev",
  p => p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddControllers()
  .ConfigureApiBehaviorOptions(options =>
  {
    options.InvalidModelStateResponseFactory = context =>
    {
      var message = string.Join("; ",
        context.ModelState.Values
          .SelectMany(v => v.Errors)
          .Select(e => e.ErrorMessage));

      if (string.IsNullOrWhiteSpace(message))
        message = "One or more validation errors occurred.";

      return new BadRequestObjectResult(new { status = 400, message });
    };
  });

builder.Services.AddOpenApi();

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("FrontendDev");

app.MapControllers();

app.Run();