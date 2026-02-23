using Common.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.API.Models;

public class PollWithVotes : IEntity<Guid>
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public int TotalVotes { get; set; } = 0;
    public List<PollOptionWithVotes> Options { get; set; } = new();
}
