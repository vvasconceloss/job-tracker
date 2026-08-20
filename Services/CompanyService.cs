using JobTracker.Data;
using JobTracker.Models;
using JobTracker.Interfaces;
using JobTracker.DTOs.Company;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Services
{
  public class CompanyService(ApplicationDbContext context) : ICompanyService
  {
    private readonly ApplicationDbContext _context = context;
    
    public async Task<IEnumerable<CompanyResponseDto>> GetAllAsync()
    {
      return await _context.Companies
        .AsNoTracking()
        .Select(c => new CompanyResponseDto(c.Id, c.Name, c.Website))
        .ToListAsync();
    }

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

    public async Task<CompanyResponseDto?> GetByIdAsync(int id)
    {
      var company = await _context.Companies.FindAsync(id);
      if (company == null) return null;

      return new CompanyResponseDto(company.Id, company.Name, company.Website);
    }
  }
}