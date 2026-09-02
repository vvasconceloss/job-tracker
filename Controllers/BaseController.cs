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
        ErrorType.NotFound => NotFound(new { status = 404, message = result.Error.Message }),
        ErrorType.Conflict => Conflict(new { status = 409, message = result.Error.Message }),
        ErrorType.Validation => BadRequest(new { status = 400, message = result.Error.Message }),
        _ => BadRequest(new { status = 400, message = result.Error.Message })
      };
    }

    protected IActionResult HandleResult(Result result)
    {
      if (result.IsSuccess)
      {
        return NoContent();
      }

      return result.Error!.Type switch
      {
        ErrorType.NotFound => NotFound(new { status = 404, message = result.Error.Message }),
        ErrorType.Conflict => Conflict(new { status = 409, message = result.Error.Message }),
        ErrorType.Validation => BadRequest(new { status = 400, message = result.Error.Message }),
        _ => BadRequest(new { status = 400, message = result.Error.Message })
      };
    }
  }
}