namespace NotificationService.API.DTOs;

public record class CreateNotificationPollRequest(Guid Id, string Title, bool IsClosed,
    List<CreateNotificationPollOptionRequest> Options, int TotalVotes = 0)
{ }
public record class CreateNotificationPollOptionRequest(Guid Id, string Text, int VoteCount = 0) { }

