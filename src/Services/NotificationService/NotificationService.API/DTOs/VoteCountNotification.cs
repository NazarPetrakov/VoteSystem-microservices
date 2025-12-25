namespace NotificationService.API.DTOs;

public class VoteCountNotification
{
    public Guid PollId { get; set; }
    public int PollVotesCount { get; set; }
    public Guid PollOptionId { get; set; }
    public int PollOptionVotesCount { get; set; }

}
