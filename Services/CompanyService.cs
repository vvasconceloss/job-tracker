using JobTracker.Data;
using JobTracker.Models;
using JobTracker.Interfaces;
using JobTracker.DTOs.Company;
using Microsoft.EntityFrameworkCore;
using JobTracker.Common;
using JobTracker.Enums;

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

    public async Task<Result<CompanyResponseDto>> CreateAsync(CreateCompanyDto dto)
    {
      var nameExists = await _context.Companies
        .AnyAsync(c => EF.Functions.ILike(c.Name, dto.Name));

      if (nameExists)
      {
        return Result<CompanyResponseDto>.Failure(ErrorType.Conflict, $"The company “{dto.Name}” already exists.");
      }

      var newCompany = new Company
      {
        Name = dto.Name,
        Website = dto.Website
      };

      _context.Companies.Add(newCompany);
      await _context.SaveChangesAsync();

      return new CompanyResponseDto(newCompany.Id, newCompany.Name, newCompany.Website);
    }

    public async Task<Result<CompanyResponseDto>> GetByIdAsync(int id)
    {
      var company = await _context.Companies.FindAsync(id);
      if (company == null) return Result<CompanyResponseDto>.Failure(ErrorType.NotFound, $"The company with ID {id} could not be found.");

      return new CompanyResponseDto(company.Id, company.Name, company.Website);
    }
  }
}