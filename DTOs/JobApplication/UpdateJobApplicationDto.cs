using JobTracker.Enums;

namespace JobTracker.DTOs.JobApplication
{
  public record UpdateJobApplicationDto(
    string Position,
    ApplicationStatus Status,
    string? Location,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string? JobUrl,
    DateTime AppliedAt,
    string? Notes
  );
}