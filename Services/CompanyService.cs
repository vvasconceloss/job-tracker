using JobTracker.Data;
using JobTracker.Models;
using JobTracker.Interfaces;
using JobTracker.DTOs.Company;

namespace JobTracker.Services
{
  public class CompanyService(ApplicationDbContext context) : ICompanyService
  {
    private readonly ApplicationDbContext _context = context;

    public async Task<CompanyResponseDto> CreateAsync(CreateCompanyDto dto)
    {
      var newCompany = new Company
      {
        Name = dto.Name,
        Website = dto.Website
      };

      _context.Companies.Add(newCompany);
      await _context.SaveChangesAsync();

      return new CompanyResponseDto(newCompany.Id, newCompany.Name, newCompany.Website);
    }

    public Task<CompanyResponseDto> GetByIdAsync(int id)
    {
      throw new NotImplementedException();
    }
  }
}