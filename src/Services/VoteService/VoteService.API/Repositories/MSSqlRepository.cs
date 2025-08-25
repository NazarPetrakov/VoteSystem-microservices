using Common.Entities;
using Common.Repositories;
using VoteService.API.Data;
namespace VoteService.API.Repositories;

public class MSSqlRepository<T, TId>(VoteDbContext context) : MSSqlRepository<T, VoteDbContext, TId>(context)
where T : class, IEntity<TId>
where TId : IEquatable<TId>
{
}
