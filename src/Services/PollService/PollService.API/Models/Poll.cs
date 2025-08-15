namespace PollService.API.Models;

public class Poll : BaseEntity
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int CreatedByUserId { get; set; }
    public bool IsClosed { get; set; }

    public ICollection<PollOption> PollOptions { get; set; } = [];
}
