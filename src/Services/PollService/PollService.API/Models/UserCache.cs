using Common.Entities;

namespace PollService.API.Models;

public class UserCache : IEntity<int>
{
    public required int Id { get; set; }
    public required string UserName { get; set; }
}
