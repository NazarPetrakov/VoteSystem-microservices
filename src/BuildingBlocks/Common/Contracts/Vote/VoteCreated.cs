namespace Common.Contracts.Vote;

public record VoteCreated(Guid VoteId, int UserId, Guid PollId, Guid PollOptionId){}
