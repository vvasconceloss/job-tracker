using JobTracker.Data;
using JobTracker.Enums;
using JobTracker.Models;
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

    public async Task<IEnumerable<JobApplicationResponseDto>> GetAllAsync(ApplicationStatus? status, int? companyId, DateTime? from, DateTime? to)
    {
      var query = _context.JobApplications.AsNoTracking().AsQueryable();

      if (status.HasValue) query = query.Where(ja => ja.Status == status.Value);
      if (companyId.HasValue) query = query.Where(ja => ja.CompanyId == companyId.Value);
      if (from.HasValue) query = query.Where(ja => ja.AppliedAt >= from.Value.Date);
      if (to.HasValue) query = query.Where(ja => ja.AppliedAt < to.Value.Date.AddDays(1));

      return await query
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

    public async Task<Result<JobApplicationResponseDto>> CreateAsync(CreateJobApplicationDto dto)
    {
      if (string.IsNullOrWhiteSpace(dto.Position))
        return Result<JobApplicationResponseDto>.Failure(ErrorType.Validation, "Position is required.");

      if (dto.SalaryMin.HasValue && dto.SalaryMin.Value < 0)
        return Result<JobApplicationResponseDto>.Failure(ErrorType.Validation, "SalaryMin cannot be negative.");

      if (dto.SalaryMax.HasValue && dto.SalaryMax.Value < 0)
        return Result<JobApplicationResponseDto>.Failure(ErrorType.Validation, "SalaryMax cannot be negative.");

      if (dto.SalaryMin.HasValue && dto.SalaryMax.HasValue && dto.SalaryMax.Value < dto.SalaryMin.Value)
        return Result<JobApplicationResponseDto>.Failure(ErrorType.Validation, "SalaryMax must be greater than or equal to SalaryMin.");

      if (dto.AppliedAt.Date > DateTime.UtcNow.Date)
        return Result<JobApplicationResponseDto>.Failure(ErrorType.Validation, "AppliedAt cannot be a future date.");

      if (!string.IsNullOrWhiteSpace(dto.JobUrl) && !IsValidUrl(dto.JobUrl))
        return Result<JobApplicationResponseDto>.Failure(ErrorType.Validation, "JobUrl must be a valid URL.");

      var company = await _context.Companies.FindAsync(dto.CompanyId);
      
      if (company == null)
        return Result<JobApplicationResponseDto>.Failure(ErrorType.NotFound, $"The company with ID {dto.CompanyId} could not be found.");

      var jobApplication = new JobApplication
      {
        CompanyId = dto.CompanyId,
        Company = company,
        Position = dto.Position,
        Status = dto.Status,
        Location = dto.Location,
        SalaryMin = dto.SalaryMin,
        SalaryMax = dto.SalaryMax,
        JobUrl = dto.JobUrl,
        AppliedAt = dto.AppliedAt,
        Notes = dto.Notes
      };

      _context.JobApplications.Add(jobApplication);
      await _context.SaveChangesAsync();

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
        new CompanyResponseDto(company.Id, company.Name, company.Website)
      );
    }

    public async Task<Result<JobApplicationResponseDto>> UpdateAsync(Guid id, UpdateJobApplicationDto dto)
    {
      var jobApplication = await _context.JobApplications.FindAsync(id);

      if (jobApplication == null)
        return Result<JobApplicationResponseDto>.Failure(ErrorType.NotFound, $"The Job Application with ID {id} could not be found.");

      if (string.IsNullOrWhiteSpace(dto.Position))
        return Result<JobApplicationResponseDto>.Failure(ErrorType.Validation, "Position is required.");

      if (dto.SalaryMin.HasValue && dto.SalaryMin.Value < 0)
        return Result<JobApplicationResponseDto>.Failure(ErrorType.Validation, "SalaryMin cannot be negative.");

      if (dto.SalaryMax.HasValue && dto.SalaryMax.Value < 0)
        return Result<JobApplicationResponseDto>.Failure(ErrorType.Validation, "SalaryMax cannot be negative.");

      if (dto.SalaryMin.HasValue && dto.SalaryMax.HasValue && dto.SalaryMax.Value < dto.SalaryMin.Value)
        return Result<JobApplicationResponseDto>.Failure(ErrorType.Validation, "SalaryMax must be greater than or equal to SalaryMin.");

      if (dto.AppliedAt.Date > DateTime.UtcNow.Date)
        return Result<JobApplicationResponseDto>.Failure(ErrorType.Validation, "AppliedAt cannot be a future date.");

      if (!string.IsNullOrWhiteSpace(dto.JobUrl) && !IsValidUrl(dto.JobUrl))
        return Result<JobApplicationResponseDto>.Failure(ErrorType.Validation, "JobUrl must be a valid URL.");

      jobApplication.Position = dto.Position;
      jobApplication.Status = dto.Status;
      jobApplication.Location = dto.Location;
      jobApplication.SalaryMin = dto.SalaryMin;
      jobApplication.SalaryMax = dto.SalaryMax;
      jobApplication.JobUrl = dto.JobUrl;
      jobApplication.AppliedAt = dto.AppliedAt;
      jobApplication.Notes = dto.Notes;
      jobApplication.UpdatedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync();

      await _context.Entry(jobApplication).Reference(ja => ja.Company).LoadAsync();
      var company = jobApplication.Company;

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
        company != null
          ? new CompanyResponseDto(company.Id, company.Name, company.Website)
          : new CompanyResponseDto(jobApplication.CompanyId, "N/A", null)
      );
    }

    private static bool IsValidUrl(string url)
    {
      return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
    
    public async Task<Result<JobApplicationResponseDto>> UpdateStatusAsync(Guid id, ApplicationStatus status) {
      var jobApplication = await _context.JobApplications.FindAsync(id);
      
      if (jobApplication == null) return Result<JobApplicationResponseDto>.Failure(ErrorType.NotFound, $"The Job Application with ID {id} could not be found.");
      if (jobApplication.Status is ApplicationStatus.Offer or ApplicationStatus.Rejected or ApplicationStatus.Withdrawn)
        return Result<JobApplicationResponseDto>.Failure(ErrorType.Validation, $"Cannot transition from terminal status '{jobApplication.Status}' via PATCH. Use PUT for manual corrections.");
      
      jobApplication.Status = status;
      jobApplication.UpdatedAt = DateTime.UtcNow;
  
      await _context.SaveChangesAsync();
      await _context.Entry(jobApplication).Reference(x => x.Company).LoadAsync();

      var company = jobApplication.Company;
      
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
        company != null
          ? new CompanyResponseDto(company.Id, company.Name, company.Website)
          : new CompanyResponseDto(jobApplication.CompanyId, "N/A", null)
      );
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