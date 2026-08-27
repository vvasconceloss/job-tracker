using JobTracker.Common;
using JobTracker.DTOs.JobApplication;

namespace JobTracker.Interfaces
{
  public interface IJobApplicationService
  {
    Task<IEnumerable<JobApplicationResponseDto>> GetAllAsync();
    Task<Result<JobApplicationResponseDto>> GetByIdAsync(Guid id);
    Task<Result<JobApplicationResponseDto>> CreateAsync(CreateJobApplicationDto dto);
    Task<Result<JobApplicationResponseDto>> UpdateAsync(Guid id, UpdateJobApplicationDto dto);
    Task<Result> DeleteAsync(Guid id);
  }
}