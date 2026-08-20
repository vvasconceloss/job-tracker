using JobTracker.Interfaces;
using JobTracker.DTOs.Company;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CompanyController(ICompanyService companyService) : ControllerBase
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
      var company = await _companyService.GetByIdAsync(id);
      if (company == null)return NotFound(new { message = $"Company with ID {id} not found." });

      return Ok(company);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyDto dto)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);
      var createdTask = await _companyService.CreateAsync(dto);

      return CreatedAtAction(nameof(GetById), createdTask);
    }
  }
}