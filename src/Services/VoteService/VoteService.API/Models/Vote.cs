using Common.Entities;

namespace VoteService.API.Models;

public class Vote : BaseEntity<Guid>
{
    public int UserId { get; set; }

    public PollCache? Poll { get; set; }
    public Guid? PollId { get; set; }
    public PollOptionCache? PollOption { get; set; }
    public Guid? PollOptionId { get; set; }
}
