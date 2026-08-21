using JobTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Data.Configurations
{
  public class CompanyConfiguration : IEntityTypeConfiguration<Company>
  {
    public void Configure(EntityTypeBuilder<Company> builder)
    {
      builder.HasKey(c => c.Id);

      builder.Property(c => c.Name)
        .IsRequired()
        .HasMaxLength(150);
      
      builder.HasIndex(c => c.Name)
        .IsUnique();

      builder.Property(c => c.CreatedAt)
        .HasDefaultValueSql("timezone('utc', now())");

      builder.Property(c => c.UpdatedAt)
        .HasDefaultValueSql("timezone('utc', now())");
    }
  }  
}

