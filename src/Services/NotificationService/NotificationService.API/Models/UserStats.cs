using System;
using Common.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.API.Models;

public class UserStats : IEntity<int>
{
    [BsonId]
    public int Id { get; set; }
    public int CreatedPollsCount { get; set; }
    public int VotedPollsCount { get; set; }

    public UserStats(int id, int createdPollsCount, int votedPollsCount)
    {
        Id = id;
        CreatedPollsCount = createdPollsCount;
        VotedPollsCount = votedPollsCount;
    }
}
