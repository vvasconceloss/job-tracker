using JobTracker.Enums;
using JobTracker.Common;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public abstract class BaseController : ControllerBase
  {
    protected IActionResult HandleResult<T>(Result<T> result)
    {
      if (result.IsSuccess)
      {
        return Ok(result.Value);
      }
      
      return result.Error!.Type switch
      {
        ErrorType.NotFound => NotFound(new { message = result.Error.Message }),
        ErrorType.Conflict => Conflict(new { message = result.Error.Message }),
        ErrorType.Validation => BadRequest(new { message = result.Error.Message }),
        _ => BadRequest(new { message = result.Error.Message })
      };
    }
  }
}