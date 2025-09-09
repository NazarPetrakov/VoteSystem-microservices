using System;
using Common.Repositories;
using Common.Tests.Data;
using Microsoft.EntityFrameworkCore;

namespace Common.Tests;

public class MSSqlRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var repo = CreateRepository(out var context);
        var entities = new[]
        {
            new TestEntity { Id = Guid.NewGuid(), Name = "A" },
            new TestEntity { Id = Guid.NewGuid(), Name = "B" }
        };
        await context.TestEntities.AddRangeAsync(entities);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repo.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, e => e.Name == "A");
        Assert.Contains(result, e => e.Name == "B");
    }
    [Fact]
    public async Task GetAsync_ShouldReturnEntityById()
    {
        // Arrange
        var repo = CreateRepository(out var context);
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Hello" };
        await context.TestEntities.AddAsync(entity, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repo.GetAsync(entity.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Name, result!.Name);
    }
    [Fact]
    public async Task UpdateAndSaveAsync_ShouldModifyEntity()
    {
        // Arrange
        var repo = CreateRepository(out var context);
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Old" };
        await context.TestEntities.AddAsync(entity, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        entity.Name = "New";
        await repo.UpdateAndSaveAsync(entity);

        var updated = await context.TestEntities
            .FirstOrDefaultAsync(e => e.Id == entity.Id, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("New", updated!.Name);
    }
    [Fact]
    public async Task DeleteAndSaveAsync_ShouldRemoveEntity()
    {
        // Arrange
        var repo = CreateRepository(out var context);
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "ToDelete" };
        await context.TestEntities.AddAsync(entity, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        await repo.DeleteAndSaveAsync(entity);
        var deleted = await context.TestEntities
            .FirstOrDefaultAsync(e => e.Id == entity.Id, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(deleted);
    }
    [Fact]
    public async Task DeleteRangeAndSaveAsync_ShouldRemoveMultipleEntities()
    {
        // Arrange
        var repo = CreateRepository(out var context);
        var entities = new[]
        {
            new TestEntity { Id = Guid.NewGuid(), Name = "A" },
            new TestEntity { Id = Guid.NewGuid(), Name = "B" }
        };
        await context.TestEntities.AddRangeAsync(entities);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        await repo.DeleteRangeAndSaveAsync(entities);
        var all = await context.TestEntities.ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Empty(all);
    }
    [Fact]
    public async Task CreateAndSaveAsync_ShouldPersistEntity()
    {
        //Arrange
        var repo = CreateRepository(out var context);
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Hello" };

        //Act
        await repo.CreateAndSaveAsync(entity);

        //Assert
        var result = await context.TestEntities.FirstOrDefaultAsync(e => e.Id == entity.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal("Hello", result.Name);
    }
    private static MSSqlRepository<TestEntity, TestDbContext, Guid> CreateRepository(out TestDbContext context)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        context = new TestDbContext(options);
        return new MSSqlRepository<TestEntity, TestDbContext, Guid>(context);
    }
}
