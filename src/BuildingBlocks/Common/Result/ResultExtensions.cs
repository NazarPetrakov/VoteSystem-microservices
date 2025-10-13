namespace Common.Result;

public static class ResultExtensions
{
    public static T Match<TValue, T>(
            this Result<TValue> result,
            Func<TValue, T> onSuccess,
            Func<List<Error>, T> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Data!) : onFailure(result.Errors);
    }
    public static T Match<T>(
            this Result result,
            Func<T> onSuccess,
            Func<List<Error>, T> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Errors);
    }
}
