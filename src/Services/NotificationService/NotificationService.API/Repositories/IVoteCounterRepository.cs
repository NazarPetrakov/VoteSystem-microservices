using NotificationService.API.Models;

namespace NotificationService.API.Repositories;

public interface IVoteCounterRepository
{
    Task<PollWithVotes> IncrementVoteCountAsync(Guid pollId, Guid optionId);
    Task<PollWithVotes> DecrementVoteCountAsync(Guid pollId, Guid optionId);
}
