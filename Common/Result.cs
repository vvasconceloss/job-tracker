using JobTracker.Enums;

namespace JobTracker.Common
{
  public class Result
  {
    public Error? Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, Error? error)
    {
        Error = error;
        IsSuccess = isSuccess;
    }

    public static Result Success() => new(true, null);
    
    public static Result Failure(ErrorType type, string message) => 
      new(false, new Error(type, message));
  }

  public class Result<T> : Result
  {
    public T? Value { get; }

    private Result(bool isSuccess, T? value, Error? error) : base(isSuccess, error)
    {
      Value = value;
    }
    
    public static Result<T> Success(T value) => new(true, value, null);
    
    public static new Result<T> Failure(ErrorType type, string message) => 
      new(false, default, new Error(type, message));
    
    public static implicit operator Result<T>(T value) => Success(value);
  }
}