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
    [ProducesResponseType(typeof(IEnumerable<JobApplicationResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ApplicationStatus? status, [FromQuery] int? companyId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
      var jobApplications = await _jobApplicationService.GetAllAsync(status, companyId, from, to);
      return Ok(jobApplications);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(JobApplicationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
      var result = await _jobApplicationService.GetByIdAsync(id);
      return HandleResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(JobApplicationResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateJobApplicationDto dto)
    {
      var result = await _jobApplicationService.CreateAsync(dto);
      if (!result.IsSuccess) return HandleResult(result);

      return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(JobApplicationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobApplicationDto dto)
    {
      var result = await _jobApplicationService.UpdateAsync(id, dto);
      return HandleResult(result);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(JobApplicationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateJobApplicationStatusDto dto) {
      var result = await _jobApplicationService.UpdateStatusAsync(id, dto.Status);
      return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
      var result = await _jobApplicationService.DeleteAsync(id);
      if (result.IsFailure) return HandleResult(result);

      return NoContent();
    }
  }
}
