using NotificationService.API.Models;

namespace NotificationService.API.Repositories;

public interface IVoteCounterRepository
{
    Task<NotificationPoll> IncrementVoteCountAsync(Guid pollId, Guid optionId);
    Task<NotificationPoll> DecrementVoteCountAsync(Guid pollId, Guid optionId);
}
