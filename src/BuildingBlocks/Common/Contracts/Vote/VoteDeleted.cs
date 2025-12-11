namespace Common.Contracts.Vote;

public record VoteDeleted(Guid VoteId, Guid PollId, Guid PollOptionId) { }
