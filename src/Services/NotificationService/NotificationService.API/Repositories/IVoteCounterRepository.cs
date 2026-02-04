using NotificationService.API.Models;

namespace NotificationService.API.Repositories;

public interface IVoteCounterRepository
{
    Task<PollWithTotalVotes> IncrementVoteCountAsync(Guid pollId, Guid optionId);
    Task<PollWithTotalVotes> DecrementVoteCountAsync(Guid pollId, Guid optionId);
}
