namespace PollService.API.Helpers;

public record CreatePollRequest(string Title, string Description,
    int UserId, bool IsClosed, ICollection<CreatePollOptionFromPollDto>? pollOptionsToCreate);
public record UpdatePollRequest(Guid Id, string? Title, string? Description,
    int? UserId, bool? IsClosed);
public record PollResponse(Guid Id, string Title, string Description,
    int UserId, string UserName, bool IsClosed,
        DateTimeOffset CreatedTime, DateTimeOffset? UpdatedTime, ICollection<PollOptionResponse> pollOptions);
