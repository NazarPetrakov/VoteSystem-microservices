using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.API.Models;

public class NotificationPoll
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsClosed { get; set; } = false;
    public int TotalVotes { get; set; } = 0;
    public List<NotificationPollOption> Options { get; set; } = new();
}
