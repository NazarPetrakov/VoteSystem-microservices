using System;
using MongoDB.Driver;
using NotificationService.API.Models;

namespace NotificationService.API.Repositories;

public class VoteCounterRepository : IVoteCounterRepository
{
    private readonly string _collectionName = "PollNotification";
    private readonly IMongoCollection<PollWithTotalVotes> _collection;

    public VoteCounterRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<PollWithTotalVotes>(_collectionName);
    }

    public async Task<PollWithTotalVotes> DecrementVoteCountAsync(Guid pollId, Guid optionId)
    {
        var filter = Builders<PollWithTotalVotes>.Filter.And(
            Builders<PollWithTotalVotes>.Filter.Eq(p => p.Id, pollId),
            Builders<PollWithTotalVotes>.Filter.ElemMatch(
                p => p.Options, o => o.OptionId == optionId && o.VoteCount > 0)
    );

        var update = Builders<PollWithTotalVotes>.Update
            .Inc(p => p.TotalVotes, -1)
            .Inc("Options.$.VoteCount", -1);

        var options = new FindOneAndUpdateOptions<PollWithTotalVotes>
        {
            ReturnDocument = ReturnDocument.After
        };

        var updatedPoll = await _collection.FindOneAndUpdateAsync(filter, update, options);

        return updatedPoll;
    }

    public async Task<PollWithTotalVotes> IncrementVoteCountAsync(Guid pollId, Guid optionId)
    {
        var filter = Builders<PollWithTotalVotes>.Filter.And(
            Builders<PollWithTotalVotes>.Filter.Eq(p => p.Id, pollId),
            Builders<PollWithTotalVotes>.Filter.ElemMatch(p => p.Options, o => o.OptionId == optionId)
        );
        var update = Builders<PollWithTotalVotes>.Update
            .Inc(p => p.TotalVotes, 1)
            .Inc("Options.$.VoteCount", 1);

        var options = new FindOneAndUpdateOptions<PollWithTotalVotes>
        {
            ReturnDocument = ReturnDocument.After
        };

        var updatedPoll = await _collection.FindOneAndUpdateAsync(filter, update, options);

        return updatedPoll;
    }
}
