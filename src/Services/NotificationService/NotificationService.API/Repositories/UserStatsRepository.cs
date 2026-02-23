using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotificationService.API.Models;
using NotificationService.API.Settings;

namespace NotificationService.API.Repositories;

public class UserStatsRepository(IMongoDatabase database, IOptions<MongoDbSettings> mongoSettings) : IUserStatsRepository
{
    private readonly IMongoCollection<UserStats> _collection = database
        .GetCollection<UserStats>(mongoSettings.Value.UserStatsCollectionName);

    public async Task IncrementCreatedPollsCountAsync(int userId) =>
        await UpdateUserStatsCounterAsync(userId, UserStatsCounter.CreatedPolls, 1);
    public async Task IncrementVotedPollsCountAsync(int userId) =>
        await UpdateUserStatsCounterAsync(userId, UserStatsCounter.VotedPolls, 1);
    public async Task DecrementCreatedPollsCountAsync(int userId) =>
        await UpdateUserStatsCounterAsync(userId, UserStatsCounter.CreatedPolls, -1);
    public async Task DecrementVotedPollsCountAsync(int userId) =>
        await UpdateUserStatsCounterAsync(userId, UserStatsCounter.VotedPolls, -1);

    private async Task UpdateUserStatsCounterAsync(int userId, UserStatsCounter userStatsCounter, int delta)
    {
        var filter = Builders<UserStats>.Filter.Eq(u => u.Id, userId);

        var update = userStatsCounter switch
        {
            UserStatsCounter.CreatedPolls => Builders<UserStats>.Update.Inc(u => u.CreatedPollsCount, delta)
                .SetOnInsert(u => u.VotedPollsCount, 0),
            UserStatsCounter.VotedPolls => Builders<UserStats>.Update.Inc(u => u.VotedPollsCount, delta)
                .SetOnInsert(u => u.CreatedPollsCount, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(userStatsCounter))
        };

        update = update
            .SetOnInsert(u => u.Id, userId);

        await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }
    private enum UserStatsCounter
    {
        CreatedPolls,
        VotedPolls
    }
}
