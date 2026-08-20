using JobTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Data
{
  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
  {
    public required DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

      modelBuilder.Entity<Company>(entity =>
      {
        entity.HasKey(c => c.Id);

        entity.Property(c => c.Id)
          .ValueGeneratedOnAdd();

        entity.HasIndex(c => c.Name)
          .IsUnique();

        entity.Property(e => e.CreatedAt)
          .HasDefaultValueSql("GETUTCDATE()");
          
        entity.Property(e => e.UpdatedAt)
          .HasDefaultValueSql("GETUTCDATE()"); 
      });
    }
  }
}