using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotificationService.API.Models;
using NotificationService.API.Settings;

namespace NotificationService.API.Repositories;

public class VoteCounterRepository(IMongoDatabase database, IOptions<MongoDbSettings> mongoSettings) : IVoteCounterRepository
{
    private readonly IMongoCollection<PollWithVotes> _collection = database
        .GetCollection<PollWithVotes>(mongoSettings.Value.PollWithTotalVotesCollectionName);

    public async Task<PollWithVotes> DecrementVoteCountAsync(Guid pollId, Guid optionId)
    {
        return await UpdateVoteCountAsync(pollId, optionId, -1);
    }
    public async Task<PollWithVotes> IncrementVoteCountAsync(Guid pollId, Guid optionId)
    {
        return await UpdateVoteCountAsync(pollId, optionId, 1);
    }
    private async Task<PollWithVotes> UpdateVoteCountAsync(Guid pollId, Guid optionId, int delta)
    {
        var filter = Builders<PollWithVotes>.Filter.And(
            Builders<PollWithVotes>.Filter.Eq(p => p.Id, pollId),
            Builders<PollWithVotes>.Filter.ElemMatch(p => p.Options, o => o.OptionId == optionId)
        );
        var update = Builders<PollWithVotes>.Update
            .Inc(p => p.TotalVotes, delta)
            .Inc("Options.$.VoteCount", delta);

        var options = new FindOneAndUpdateOptions<PollWithVotes>
        {
            ReturnDocument = ReturnDocument.After
        };

        var updatedPoll = await _collection.FindOneAndUpdateAsync(filter, update, options);

        return updatedPoll;
    }
}
