using Common.Result;

namespace Common.Errors;

public static class PollErrors
{
    public static Error NotFound(Guid id) => new("Polls.NotFound", $"The poll with Id '{id}' not found");
}
public static class PollOptionErrors
{
    public static Error NotFound(Guid id) => new("PollOptions.NotFound",
        $"The poll option with Id '{id}' not found");
}
