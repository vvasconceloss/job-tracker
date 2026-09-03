using JobTracker.Interfaces;
using JobTracker.DTOs.Company;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Controllers
{
  [ApiController]
  [Route("api/companies")]
  public class CompaniesController(ICompanyService companyService) : BaseController
  {
    private readonly ICompanyService _companyService = companyService;

      [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CompanyResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
      var companies = await _companyService.GetAllAsync();
      return Ok(companies);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CompanyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
      var result = await _companyService.GetByIdAsync(id);
      return HandleResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CompanyResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateCompanyDto dto)
    {
      var result = await _companyService.CreateAsync(dto);
      if (!result.IsSuccess) return HandleResult(result);

      return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }
  }
}