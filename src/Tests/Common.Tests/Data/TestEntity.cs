using Common.Entities;

namespace Common.Tests.Data;

public class TestEntity : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
}