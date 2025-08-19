namespace Common.Entities;

public class BaseEntity<TId> : IEntity<TId>, IHasTimestamps where TId : IEquatable<TId>
{
    public TId Id { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
}
