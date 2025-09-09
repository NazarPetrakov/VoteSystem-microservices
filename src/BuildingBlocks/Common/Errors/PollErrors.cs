using Common.Result;

namespace Common.Errors;

public static class PollErrors
{
    public static Error NotFound(Guid id) => new("Polls.NotFound", $"The poll with Id '{id}' not found.");
    public static Error MissingOption(Guid pollId, Guid pollOptionId) => new("Polls.MissingOption",
        $"The poll with Id '{pollId}' does not contain the option with Id '{pollOptionId}'.");
    public static Error AlreadyClosed(Guid pollId) => new("Polls.AlreadyClosed", $"The poll with Id '{pollId}' is already closed.");
}
public static class PollOptionErrors
{
    public static Error NotFound(Guid id) => new("PollOptions.NotFound",
        $"The poll option with Id '{id}' not found.");
}
