namespace Common.Result;

public class Result<T> : Result
{
    public Result(bool isSuccess, List<Error> errors, T? data) : base(isSuccess, errors)
    {
        Data = data;
    }
    public T? Data { get; }
}
public class Result
{
    protected Result(bool isSuccess, List<Error> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public bool IsSuccess { get; }
    public List<Error> Errors { get; }

    public static Result Success() => new(true, new());
    public static Result Failure(Error error) => new(false, new() { error });
    public static Result Failure(IEnumerable<Error> errors) => new(false, errors.ToList());

    public static Result<T> Success<T>(T data) => new(true, new(), data);
    public static Result<T> Failure<T>(Error error) => new(false, new() { error }, default);
    public static Result<T> Failure<T>(IEnumerable<Error> errors) => new(false, errors.ToList(), default);
}
