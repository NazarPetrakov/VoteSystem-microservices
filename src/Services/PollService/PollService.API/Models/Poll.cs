using Common.Entities;

namespace PollService.API.Models;

public class Poll : BaseEntity<Guid>
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public bool IsClosed { get; set; }

    public int UserId { get; set; }
    public UserCache User { get; set; } = null!;
    public ICollection<PollOption> PollOptions { get; set; } = [];
}
