using JobTracker.DTOs.Dashboard;

namespace JobTracker.Interfaces
{
  public interface IDashboardService
  {
    Task<DashboardResponseDto> GetDashboardAsync();
  }
}