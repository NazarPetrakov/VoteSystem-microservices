using Common.Entities;

namespace PollService.API.Models;

public class PollOption : BaseEntity
{
    public required string Text { get; set; }
    public int Order { get; set; }

    public Poll Poll { get; set; } = null!;
    public Guid PollId { get; set; }

}
