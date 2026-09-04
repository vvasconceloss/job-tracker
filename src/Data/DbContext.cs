using JobTracker.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Data
{
  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
  {
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<JobApplication> JobApplications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
  }
}