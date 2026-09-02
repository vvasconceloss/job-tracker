using System.Net;
using System.Text.Json;

namespace JobTracker.Middlewares;

public sealed class ExceptionHandlingMiddleware(
  RequestDelegate next,
  ILogger<ExceptionHandlingMiddleware> logger,
  IHostEnvironment env)
{
  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await next(context);
    }
    catch (Exception ex)
    {
      logger.LogError(ex,
        "Unhandled exception at {Method} {Path} TraceId {TraceIdentifier}",
        context.Request.Method,
        context.Request.Path,
        context.TraceIdentifier);

      await HandleExceptionAsync(context, ex);
    }
  }

  private Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    context.Response.ContentType = "application/json";

    var statusCode = exception switch
    {
      ArgumentException or InvalidOperationException => HttpStatusCode.BadRequest,
      KeyNotFoundException => HttpStatusCode.NotFound,
      _ => HttpStatusCode.InternalServerError
    };

    context.Response.StatusCode = (int)statusCode;

    var message = env.IsDevelopment()
      ? exception.Message
      : statusCode == HttpStatusCode.InternalServerError
        ? "An unexpected error occurred."
        : exception.Message;

    object payload = env.IsDevelopment()
      ? new { status = (int)statusCode, message, detail = exception.ToString() }
      : new { status = (int)statusCode, message };

    var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });

    return context.Response.WriteAsync(json);
  }
}
