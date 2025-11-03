using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace PollService.API.Models;

public class Poll : BaseEntity<Guid>
{
    public required string Title { get; set; }
    public string? Topic { get; set; }
    public bool IsClosed { get; set; }
    public long DurationInSeconds { get; set; }

    public int UserId { get; set; }
    public UserCache User { get; set; } = null!;
    public ICollection<PollOption> PollOptions { get; set; } = [];

    [NotMapped]
    public TimeSpan Duration
    {
        get => TimeSpan.FromSeconds(DurationInSeconds);
        set => DurationInSeconds = (long)value.TotalSeconds;
    }

    [NotMapped]
    public DateTime EndTime => CreatedAt.UtcDateTime.Add(Duration);

    [NotMapped]
    public bool IsExpired => DateTime.UtcNow > EndTime;

    [NotMapped]
    public bool IsActive => !IsClosed && !IsExpired;
}
