namespace PollService.API.Helpers;

public record CreatePollRequest(
    string Title,
    string? Topic,
    int TimeCount,
    string TimeUnit,
    int UserId,
    bool IsClosed,
    ICollection<CreatePollOptionFromPollDto>? pollOptionsToCreate);
public record UpdatePollRequest(
    Guid Id,
    string? Title,
    string? Topic,
    int? UserId,
    bool? IsClosed);
public record PollResponse(
    Guid Id,
    string Title,
    string? Topic,
    int UserId,
    string UserName,
    bool IsClosed,
    TimeSpan Duration,
    DateTimeOffset CreatedTime,
    DateTimeOffset? UpdatedTime,
    DateTime EndTime,
    bool IsExpired,
    bool IsActive,
    ICollection<PollOptionResponse> pollOptions);
