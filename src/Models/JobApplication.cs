using JobTracker.Enums;

namespace JobTracker.Models
{
  public class JobApplication
  {
    public Guid Id { get; set; }
    public int CompanyId { get; set; }
    public required Company Company { get; set; }
    public required string Position { get; set; }
    public ApplicationStatus Status { get; set; }
    public string? Location { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string? JobUrl { get; set; }
    public DateTime AppliedAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  }
}