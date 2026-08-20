using JobTracker.DTOs.Company;

namespace JobTracker.Interfaces
{
  public interface ICompanyService
  {
    Task<CompanyResponseDto?> GetByIdAsync(int id);
    Task<CompanyResponseDto> CreateAsync(CreateCompanyDto dto);
  }
}