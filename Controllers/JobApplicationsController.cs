using JobTracker.Interfaces;
using Microsoft.AspNetCore.Mvc;
using JobTracker.DTOs.JobApplication;
using JobTracker.Enums;

namespace JobTracker.Controllers
{
  [ApiController]
  [Route("api/applications")]
  public class JobApplicationsController(IJobApplicationService jobApplicationService) : BaseController
  {
    private readonly IJobApplicationService _jobApplicationService = jobApplicationService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ApplicationStatus? status, [FromQuery] int? companyId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
      var jobApplications = await _jobApplicationService.GetAllAsync(status, companyId, from, to);
      return Ok(jobApplications);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
      var result = await _jobApplicationService.GetByIdAsync(id);
      return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJobApplicationDto dto)
    {
      var result = await _jobApplicationService.CreateAsync(dto);
      if (!result.IsSuccess) return HandleResult(result);

      return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobApplicationDto dto)
    {
      var result = await _jobApplicationService.UpdateAsync(id, dto);
      return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
      var result = await _jobApplicationService.DeleteAsync(id);
      if (result.IsFailure)
      {
        return result.Error!.Type switch
        {
          Enums.ErrorType.NotFound => NotFound(new { message = result.Error.Message }),
          Enums.ErrorType.Validation => BadRequest(new { message = result.Error.Message }),
          Enums.ErrorType.Conflict => Conflict(new { message = result.Error.Message }),
          _ => BadRequest(new { message = result.Error.Message })
        };
      }

      return NoContent();
    }
  }
}
