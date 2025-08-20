using Common.Entities;
using Common.Repositories;
using PollService.API.Data;

namespace PollService.API.Repositories;

public class MSSqlRepository<T, TId>(PollDbContext context)
    : MSSqlRepository<T, PollDbContext, TId>(context)
    where T : BaseEntity<TId>
    where TId : IEquatable<TId>
{ }
