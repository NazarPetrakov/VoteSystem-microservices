using Common.Entities;
using Common.Repositories;
using PollService.API.Data;

namespace PollService.API.Repositories;

public class MSSqlRepository<T>(PollDbContext context)
    : MSSqlRepository<T, PollDbContext>(context)
    where T : BaseEntity
{ }
