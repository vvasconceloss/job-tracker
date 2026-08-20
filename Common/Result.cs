using JobTracker.Enums;

namespace JobTracker.Common
{
  public class Result<T>
  {
    public T? Value { get; }
    public Error? Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    private Result(bool isSuccess, T? value, Error? error)
    {
      Error = error;
      Value = value;
      IsSuccess = isSuccess;
    }
    
    public static Result<T> Success(T value) => new(true, value, null);
    
    public static Result<T> Failure(ErrorType type, string message) => new(false, default, new Error(type, message));
    
    public static implicit operator Result<T>(T value) => Success(value);
  }
}