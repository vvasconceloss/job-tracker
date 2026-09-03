using JobTracker.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Controllers
{
  [ApiController]
  [Route("api/dashboard")]
  public class DashboardController(IDashboardService dashboardService) : ControllerBase
  {
    [HttpGet]
    public async Task<IActionResult> Get()
    {
      return Ok(await dashboardService.GetDashboardAsync());
    }
  }
}