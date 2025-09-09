using Common.Result;

namespace Common.Errors;

public static class VoteErrors
{
    public static Error NotFound(Guid id) => new("Votes.NotFound", $"The vote with Id '{id}' not found.");
    public static Error ClosedPoll(Guid pollId) => new("Votes.ClosedPoll", $"The poll with Id '{pollId}' is closed.");
}
