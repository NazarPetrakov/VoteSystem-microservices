using System;
using MongoDB.Driver;
using NotificationService.API.Models;

namespace NotificationService.API.Repositories;

public class VoteCounterRepository : IVoteCounterRepository
{
    private readonly string _collectionName = "PollNotification";
    private readonly IMongoCollection<NotificationPoll> _collection;

    public VoteCounterRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<NotificationPoll>(_collectionName);
    }

    public async Task<NotificationPoll> DecrementVoteCountAsync(Guid pollId, Guid optionId)
    {
        var filter = Builders<NotificationPoll>.Filter.And(
            Builders<NotificationPoll>.Filter.Eq(p => p.Id, pollId),
            Builders<NotificationPoll>.Filter.ElemMatch(
                p => p.Options, o => o.OptionId == optionId && o.VoteCount > 0)
    );

        var update = Builders<NotificationPoll>.Update
            .Inc(p => p.TotalVotes, -1)
            .Inc("Options.$.VoteCount", -1);

        var options = new FindOneAndUpdateOptions<NotificationPoll>
        {
            ReturnDocument = ReturnDocument.After
        };

        var updatedPoll = await _collection.FindOneAndUpdateAsync(filter, update, options);

        return updatedPoll;
    }

    public async Task<NotificationPoll> IncrementVoteCountAsync(Guid pollId, Guid optionId)
    {
        var filter = Builders<NotificationPoll>.Filter.And(
            Builders<NotificationPoll>.Filter.Eq(p => p.Id, pollId),
            Builders<NotificationPoll>.Filter.ElemMatch(p => p.Options, o => o.OptionId == optionId)
        );
        var update = Builders<NotificationPoll>.Update
            .Inc(p => p.TotalVotes, 1)
            .Inc("Options.$.VoteCount", 1);

        var options = new FindOneAndUpdateOptions<NotificationPoll>
        {
            ReturnDocument = ReturnDocument.After
        };

        var updatedPoll = await _collection.FindOneAndUpdateAsync(filter, update, options);

        return updatedPoll;
    }
}
