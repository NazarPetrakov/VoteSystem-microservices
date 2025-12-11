namespace Common.Contracts.Vote;

public record VoteCreated(Guid VoteId, Guid PollId, Guid PollOptionId){}
