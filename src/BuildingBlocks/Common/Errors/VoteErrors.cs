using Common.Result;

namespace Common.Errors;

public static class VoteErrors
{
    public static Error NotFound(Guid id) => new("Votes.NotFound", $"The vote with Id '{id}' not found");
}
