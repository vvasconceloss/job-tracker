using JobTracker.DTOs.Company;

namespace JobTracker.DTOs.JobApplication
{
  public record JobApplicationResponseDto(
    Guid Id,
    string Position,
    string Status,
    string? Location,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string? JobUrl,
    DateTime AppliedAt,
    string? Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    CompanyResponseDto Company
  );
}