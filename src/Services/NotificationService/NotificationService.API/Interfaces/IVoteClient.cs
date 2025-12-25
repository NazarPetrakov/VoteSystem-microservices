using NotificationService.API.DTOs;

namespace NotificationService.API.Interfaces;

public interface IVoteClient
{
    Task ReceiveVotes(VoteCountNotification voteCountNotification);
}
