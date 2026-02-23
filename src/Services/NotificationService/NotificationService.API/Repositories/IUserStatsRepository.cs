
namespace NotificationService.API.Repositories;

public interface IUserStatsRepository
{
    Task IncrementCreatedPollsCountAsync(int userId);
    Task IncrementVotedPollsCountAsync(int userId);
    Task DecrementCreatedPollsCountAsync(int userId);
    Task DecrementVotedPollsCountAsync(int userId);
}
