using JobTracker.Interfaces;
using JobTracker.DTOs.Company;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CompanyController(ICompanyService companyService) : BaseController
  {
    private readonly ICompanyService _companyService = companyService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var companies = await _companyService.GetAllAsync();
      return Ok(companies);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      var result = await _companyService.GetByIdAsync(id);
      return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyDto dto)
    {
      var result = await _companyService.CreateAsync(dto);
      if (!result.IsSuccess) return HandleResult(result);

      return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }
  }
}