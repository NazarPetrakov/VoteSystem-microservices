namespace VoteService.API.Contracts.Dtos;

public record VoteResponse(Guid VoteId, int UserId, Guid? PollId, Guid? PollOptionId) { }
public record CreateVoteRequest(int UserId, Guid PollId, Guid PollOptionId) { }
