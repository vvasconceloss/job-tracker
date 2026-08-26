using JobTracker.Data;
using JobTracker.Enums;
using JobTracker.Common;
using JobTracker.Interfaces;
using JobTracker.DTOs.Company;
using Microsoft.EntityFrameworkCore;
using JobTracker.DTOs.JobApplication;

namespace JobTracker.Services
{
  public class JobApplicationService(ApplicationDbContext context) : IJobApplicationService
  {
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<JobApplicationResponseDto>> GetAllAsync()
    {
      return await _context.JobApplications
        .Select(ja => new JobApplicationResponseDto(
          ja.Id,
          ja.Position,
          ja.Status.ToString(),
          ja.Location,
          ja.SalaryMin,
          ja.SalaryMax,
          ja.JobUrl,
          ja.AppliedAt,
          ja.Notes,
          ja.CreatedAt,
          ja.UpdatedAt,
          ja.Company != null 
            ? new CompanyResponseDto(ja.Company.Id, ja.Company.Name, ja.Company.Website) 
            : new CompanyResponseDto(0, "N/A", null) 
        ))
        .ToListAsync();
    }

    public async Task<Result<JobApplicationResponseDto>> GetByIdAsync(Guid id)
    {
      var jobApplication = await _context.JobApplications.FindAsync(id);
      if (jobApplication == null) return Result<JobApplicationResponseDto>.Failure(ErrorType.NotFound, $"The Job Application with ID {id} could not be found.");

      return new JobApplicationResponseDto(
        jobApplication.Id,
        jobApplication.Position,
        jobApplication.Status.ToString(),
        jobApplication.Location,
        jobApplication.SalaryMin,
        jobApplication.SalaryMax,
        jobApplication.JobUrl,
        jobApplication.AppliedAt,
        jobApplication.Notes,
        jobApplication.CreatedAt,
        jobApplication.UpdatedAt,
        jobApplication.Company != null 
          ? new CompanyResponseDto(jobApplication.Company.Id, jobApplication.Company.Name, jobApplication.Company.Website) 
          : new CompanyResponseDto(0, "N/A", null) 
      );
    }

    public Task<Result<JobApplicationResponseDto>> CreateAsync(CreateJobApplicationDto dto)
    {
      throw new NotImplementedException();
    }

    public Task<Result<JobApplicationResponseDto>> UpdateAsync(UpdateJobApplicationDto dto)
    {
      throw new NotImplementedException();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
      var jobApplication = await _context.JobApplications.FindAsync(id);
      if (jobApplication == null) return Result.Failure(ErrorType.NotFound, $"The Job Application with ID {id} could not be found.");

      _context.JobApplications.Remove(jobApplication);
      await _context.SaveChangesAsync();
      
      return Result.Success();
    }
  }
}