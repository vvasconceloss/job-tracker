using JobTracker.Enums;

namespace JobTracker.DTOs.JobApplication
{
  public record CreateJobApplicationDto(
    int CompanyId,
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