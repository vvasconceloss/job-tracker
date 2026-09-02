using JobTracker.Common;
using JobTracker.DTOs.JobApplication;
using JobTracker.Enums;

namespace JobTracker.Interfaces
{
  public interface IJobApplicationService
  {
    Task<IEnumerable<JobApplicationResponseDto>> GetAllAsync(ApplicationStatus? status, int? companyId, DateTime? from, DateTime? to);
    Task<Result<JobApplicationResponseDto>> GetByIdAsync(Guid id);
    Task<Result<JobApplicationResponseDto>> CreateAsync(CreateJobApplicationDto dto);
    Task<Result<JobApplicationResponseDto>> UpdateAsync(Guid id, UpdateJobApplicationDto dto);
    Task<Result<JobApplicationResponseDto>> UpdateStatusAsync(Guid id, ApplicationStatus status);
    Task<Result> DeleteAsync(Guid id);
  }
}