using Common.Entities;

namespace VoteService.API.Models;

public class UserCache : IEntity<int>
{
    public int Id { get; set; }
    public required string UserName { get; set; }
}
