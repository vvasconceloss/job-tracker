using JobTracker.Data;
using JobTracker.Common;
using JobTracker.Interfaces;
using JobTracker.DTOs.JobApplication;

namespace JobTracker.Services
{
  public class JobApplicationService(ApplicationDbContext context) : IJobApplicationService
  {
    private readonly ApplicationDbContext _context = context;

    public Task<IEnumerable<JobApplicationResponseDto>> GetAllAsync()
    {
      throw new NotImplementedException();
    }

    public Task<Result<JobApplicationResponseDto>> GetByIdAsync(int id)
    {
      throw new NotImplementedException();
    }

    public Task<Result<JobApplicationResponseDto>> CreateAsync(CreateJobApplicationDto dto)
    {
      throw new NotImplementedException();
    }

    public Task<Result<JobApplicationResponseDto>> UpdateAsync(UpdateJobApplicationDto dto)
    {
      throw new NotImplementedException();
    }

    public Task<Result> DeleteAsync(Guid id)
    {
      throw new NotImplementedException();
    }
  }
}