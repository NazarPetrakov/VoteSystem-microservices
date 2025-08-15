namespace PollService.API.Helpers;

public record CreatePollOptionRequest(string Text, Guid PollId);
public record CreatePollOptionFromPollDto(string Text);
public record UpdatePollOptionRequest(Guid Id, string Text, int Order, Guid PollId);
public record PollOptionResponse(Guid Id, string Text, int Order, Guid PollId,
    DateTimeOffset CreatedTime, DateTimeOffset? UpdatedTime);
