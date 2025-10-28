using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace PollService.API.Models;

public class Poll : BaseEntity<Guid>
{
    public required string Title { get; set; }
    public string? Topic { get; set; }
    public bool IsClosed { get; set; }
    public TimeSpan Duration { get; set; }

    [NotMapped]
    public DateTime EndTime => CreatedAt.UtcDateTime.Add(Duration);

    [NotMapped]
    public bool IsExpired => DateTime.UtcNow > EndTime;

    [NotMapped]
    public bool IsActive => !IsClosed && !IsExpired;

    public int UserId { get; set; }
    public UserCache User { get; set; } = null!;
    public ICollection<PollOption> PollOptions { get; set; } = [];
}
