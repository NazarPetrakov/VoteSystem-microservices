using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.API.Models;

public class NotificationPollOption
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    // public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int VoteCount { get; set; } = 0;
}
