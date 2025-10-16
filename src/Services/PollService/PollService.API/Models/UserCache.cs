using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace PollService.API.Models;

public class UserCache : IEntity<int>
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int Id { get; set; }
    public required string UserName { get; set; }
}
