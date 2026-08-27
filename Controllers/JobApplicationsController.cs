using JobTracker.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class JobApplicationsController(IJobApplicationService jobApplicationService) : BaseController
  {
    private readonly IJobApplicationService _jobApplicationService = jobApplicationService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var jobApplications = await _jobApplicationService.GetAllAsync();
      return Ok(jobApplications);
    }
  }
}