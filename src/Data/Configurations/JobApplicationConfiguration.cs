using JobTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Data.Configurations
{
  public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
  {
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
      builder.HasKey(ja => ja.Id);
      
      builder.Property(ja => ja.Id)
        .HasDefaultValueSql("gen_random_uuid()"); 
        
      builder.Property(ja => ja.Position)
        .IsRequired()
        .HasMaxLength(100);

      builder.Property(ja => ja.Status)
        .IsRequired()
        .HasConversion<string>();

      builder.Property(ja => ja.SalaryMin)
        .HasPrecision(18, 2);

      builder.Property(ja => ja.SalaryMax)
        .HasPrecision(18, 2);

      builder.HasOne(ja => ja.Company)
        .WithMany(c => c.JobApplications)
        .HasForeignKey(ja => ja.CompanyId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.Property(ja => ja.CreatedAt)
        .HasDefaultValueSql("timezone('utc', now())");

      builder.Property(ja => ja.UpdatedAt)
        .HasDefaultValueSql("timezone('utc', now())");
    }
  }
}