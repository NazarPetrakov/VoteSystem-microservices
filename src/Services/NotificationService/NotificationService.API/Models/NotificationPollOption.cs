namespace NotificationService.API.Models;

public class NotificationPollOption
{
    public Guid OptionId { get; set; }
    public int VoteCount { get; set; } = 0;
}
