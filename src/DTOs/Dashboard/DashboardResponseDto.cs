namespace JobTracker.DTOs.Dashboard
{
  public record DashboardResponseDto(
    int TotalApplications,
    int ApplicationsThisMonth,
    int Interviews,
    int TechnicalTests,
    int Offers,
    int Rejections,
    int Withdrawn,
    double InterviewRate,
    Dictionary<string, int> ApplicationsByStatus,
    List<ApplicationsByCompanyDto> ApplicationsByCompany
  );
}