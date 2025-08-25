namespace Common.Entities;

public interface IEntity<TId> where TId : IEquatable<TId>
{
    TId Id { get; set; }
}
