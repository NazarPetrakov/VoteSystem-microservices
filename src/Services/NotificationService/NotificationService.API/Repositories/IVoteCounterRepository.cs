namespace NotificationService.API.Repositories;

public interface IVoteCounterRepository
{
    Task IncrementVoteCountAsync(Guid pollId, Guid optionId);
    Task DecrementVoteCountAsync(Guid pollId, Guid optionId);
}
