using JobTracker.Data;
using JobTracker.DTOs.Dashboard;
using JobTracker.Enums;
using JobTracker.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Services
{
  public class DashboardService(ApplicationDbContext context) : IDashboardService
  {
    private readonly ApplicationDbContext _context = context;

    public async Task<DashboardResponseDto> GetDashboardAsync()
    {
      var query = _context.JobApplications.AsNoTracking();

      var total = await query.CountAsync();

      var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0,0,0, DateTimeKind.Utc);
      var thisMonth = await query.Where(a => a.AppliedAt >= firstDayOfMonth)
        .CountAsync();

      var interviews = await query.Where(a => a.Status == ApplicationStatus.Interview)
        .CountAsync();

      var technicalTests = await query.Where(a => a.Status == ApplicationStatus.TechnicalTest)
        .CountAsync();

      var offers = await query.Where(a => a.Status == ApplicationStatus.Offer)
        .CountAsync();

      var rejections = await query.Where(a => a.Status == ApplicationStatus.Rejected)
        .CountAsync();

      var withdrawn = await query.Where(a => a.Status == ApplicationStatus.Withdrawn)
        .CountAsync();

      double rate = total == 0 ? 0 : Math.Round((double)interviews / total * 100, 1);
      
      var byStatus = await query
        .GroupBy(a => a.Status)
        .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
        .ToDictionaryAsync(x => x.Status, x => x.Count);

      var byCompany = await query
        .Include(a => a.Company)
        .GroupBy(a => a.Company.Name)
        .Select(g => new { CompanyName = g.Key, Count = g.Count() }) 
        .OrderByDescending(x => x.Count)
        .ToListAsync();

      var byCompanyDto = byCompany
        .Select(x => new ApplicationsByCompanyDto(x.CompanyName, x.Count))
        .ToList();

      return new DashboardResponseDto(
        total,
        thisMonth,
        interviews,
        technicalTests,
        offers,
        rejections,
        withdrawn,
        rate,
        byStatus,
        byCompanyDto
      );
    }
  }
}