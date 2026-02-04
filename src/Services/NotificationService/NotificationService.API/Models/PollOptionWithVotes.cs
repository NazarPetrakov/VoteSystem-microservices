namespace NotificationService.API.Models;

public class PollOptionWithVotes
{
    public Guid OptionId { get; set; }
    public int VoteCount { get; set; } = 0;
}
