using Common.Entities;
using Common.Repositories;
using NotificationService.API.Data;

namespace NotificationService.API.Repositories;

public class MongoRepository<T, TId>(AppDbContext context) : MongoRepository<T, AppDbContext, TId>(context)
    where T : class, IEntity<TId>
    where TId : IEquatable<TId>
{
}
