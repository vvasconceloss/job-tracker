using JobTracker.Common;
using JobTracker.DTOs.Company;

namespace JobTracker.Interfaces
{
  public interface ICompanyService
  {
    Task<IEnumerable<CompanyResponseDto>> GetAllAsync();
    Task<Result<CompanyResponseDto>> GetByIdAsync(int id);
    Task<Result<CompanyResponseDto>> CreateAsync(CreateCompanyDto dto);
  }
}