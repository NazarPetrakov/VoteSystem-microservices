using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.API.Models;

public class NotificationPollOption
{
    public Guid OptionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int VoteCount { get; set; } = 0;
}
