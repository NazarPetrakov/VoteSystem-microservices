namespace NotificationService.API.DTOs;

public record class CreatePollWithTotalVotesRequest(Guid Id, string Title, bool IsClosed,
    List<CreatePollOptionWithVotesRequest> Options, int TotalVotes = 0)
{ }
public record class CreatePollOptionWithVotesRequest(Guid Id, string Text, int VoteCount = 0) { }

