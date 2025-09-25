namespace NotificationService.API.DTOs;

public record class CreateNotificationPollRequest(string Title, bool IsClosed, int TotalVotes = 0)
{

}
