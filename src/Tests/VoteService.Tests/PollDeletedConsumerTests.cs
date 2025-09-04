using System.Linq.Expressions;
using Common.Contracts.Poll;
using Common.Repositories;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VoteService.API.Consumers;
using VoteService.API.Models;
using VoteService.Tests.Factories;

namespace VoteService.Tests;

public class PollDeletedConsumerTests
{
    [Fact]
    public async Task ShouldConsume_PollDeleted_DeletesPollAndItsVotes()
    {
        // Arrange
        var pollRepositoryMock = new Mock<IRepository<PollCache, Guid>>();
        var voteRepositoryMock = new Mock<IRepository<Vote, Guid>>();

        await using var provider = new ServiceCollection()
            .AddSingleton(pollRepositoryMock.Object)
            .AddSingleton(voteRepositoryMock.Object)
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<PollDeletedConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        try
        {
            var bus = provider.GetRequiredService<IBus>();
            var poll = VoteDataFactory.CreatePoll();
            var votes = VoteDataFactory.CreateVotes(poll.Id, poll.PollOptions.First().Id);

            pollRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()))
                .ReturnsAsync(poll);
            voteRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
                .ReturnsAsync(votes);
            voteRepositoryMock.Setup(r => r.DeleteRangeAndSaveAsync(It.IsAny<Vote[]>()))
                .Returns(Task.CompletedTask);
            pollRepositoryMock.Setup(r => r.DeleteAndSaveAsync(It.IsAny<PollCache>()))
                .Returns(Task.CompletedTask);

            // Act
            await bus.Publish(new PollDeleted(poll.Id), TestContext.Current.CancellationToken);

            // Assert
            Assert.True(await harness.Published.Any<PollDeleted>(TestContext.Current.CancellationToken));
            Assert.True(await harness.Consumed.Any<PollDeleted>(TestContext.Current.CancellationToken));

            var consumerHarness = harness.GetConsumerHarness<PollDeletedConsumer>();
            Assert.True(await consumerHarness.Consumed.Any<PollDeleted>(TestContext.Current.CancellationToken));

            pollRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()), Times.Once);
            voteRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<Expression<Func<Vote, bool>>>()), Times.Once);
            voteRepositoryMock.Verify(r => r.DeleteRangeAndSaveAsync(It.IsAny<Vote[]>()), Times.Once);
            pollRepositoryMock.Verify(r => r.DeleteAndSaveAsync(It.IsAny<PollCache>()), Times.Once);
        }
        finally
        {
            await harness.Stop(TestContext.Current.CancellationToken);
        }
    }
    [Fact]
    public async Task PollDeletedConsumer_WhenPollNotFound_ReturnsFault()
    {
        // Arrange
        var pollRepositoryMock = new Mock<IRepository<PollCache, Guid>>();
        var voteRepositoryMock = new Mock<IRepository<Vote, Guid>>();

        await using var provider = new ServiceCollection()
            .AddSingleton(pollRepositoryMock.Object)
            .AddSingleton(voteRepositoryMock.Object)
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<PollDeletedConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        try
        {
            var bus = provider.GetRequiredService<IBus>();
            var poll = VoteDataFactory.CreatePoll();

            pollRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()))
                .ReturnsAsync((PollCache?)null);

            // Act
            await bus.Publish(new PollDeleted(poll.Id), TestContext.Current.CancellationToken);

            // Assert
            Assert.True(await harness.Published.Any<Fault<PollDeleted>>(TestContext.Current.CancellationToken));
        }
        finally
        {
            await harness.Stop(TestContext.Current.CancellationToken);
        }
    }
}
