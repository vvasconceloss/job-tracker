using JobTracker.Enums;

namespace JobTracker.Common
{
  public record Error(ErrorType Type, string Message);
}