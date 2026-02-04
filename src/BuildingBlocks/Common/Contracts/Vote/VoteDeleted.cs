namespace Common.Contracts.Vote;

public record VoteDeleted(Guid VoteId, int UserId, Guid PollId, Guid PollOptionId) { }
