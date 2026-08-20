using JobTracker.Data;
using JobTracker.DTOs.Company;
using JobTracker.Interfaces;

namespace JobTracker.Services
{
  public class CompanyService(ApplicationDbContext context) : ICompanyService
  {
    private readonly ApplicationDbContext _context = context;

    public Task<CompanyResponseDto> CreateAsync(CreateCompanyDto dto)
    {
      throw new NotImplementedException();
    }

    public Task<CompanyResponseDto> GetByIdAsync(int id)
    {
      throw new NotImplementedException();
    }
  }
}