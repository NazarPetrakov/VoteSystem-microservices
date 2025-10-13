using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace VoteService.API.Models;

public class UserCache : IEntity<int>
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public required string UserName { get; set; }
}
