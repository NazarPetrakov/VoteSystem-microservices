using Common.Entities;

namespace VoteService.API.Models;

public class PollCache : IEntity<Guid>
{
    public Guid Id { get; set; }
    public bool IsClosed { get; set; }

    public ICollection<PollOptionCache> PollOptions { get; set; } = [];
    public ICollection<Vote> Votes { get; set; } = [];
}
