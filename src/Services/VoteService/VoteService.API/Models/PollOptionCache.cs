using Common.Entities;

namespace VoteService.API.Models;

public class PollOptionCache : IEntity<Guid>
{
    public Guid Id { get; set; }

    public Guid PollId { get; set; }
    public PollCache Poll { get; set; } = null!;
    public ICollection<Vote> Votes { get; set; } = [];

}
