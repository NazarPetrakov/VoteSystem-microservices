using System.Linq.Expressions;
using Common.Contracts.PollOption;
using Common.Repositories;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VoteService.API.Consumers;
using VoteService.API.Models;
using VoteService.Tests.Factories;

namespace VoteService.Tests;

public class PollOptionDeletedConsumerTests
{
    [Fact]
    public async Task ShouldConsume_PollDeleted_DeletesPollAndItsVotes()
    {
        // Arrange
        var pollOptionRepositoryMock = new Mock<IRepository<PollOptionCache, Guid>>();
        var voteRepositoryMock = new Mock<IRepository<Vote, Guid>>();

        await using var provider = new ServiceCollection()
            .AddSingleton(pollOptionRepositoryMock.Object)
            .AddSingleton(voteRepositoryMock.Object)
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<PollOptionDeletedConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        try
        {
            var bus = provider.GetRequiredService<IBus>();
            var poll = VoteDataFactory.CreatePoll();
            var votes = VoteDataFactory.CreateVotes(poll.Id, poll.PollOptions.First().Id);

            pollOptionRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOptionCache, object>>>()))
                .ReturnsAsync(poll.PollOptions.First());
            voteRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
                .ReturnsAsync(votes);
            voteRepositoryMock.Setup(r => r.DeleteRangeAndSaveAsync(It.IsAny<Vote[]>()))
                .Returns(Task.CompletedTask);
            pollOptionRepositoryMock.Setup(r => r.DeleteAndSaveAsync(It.IsAny<PollOptionCache>()))
                .Returns(Task.CompletedTask);

            // Act
            await bus.Publish(new PollOptionDeleted(poll.PollOptions.First().Id), TestContext.Current.CancellationToken);

            // Assert
            Assert.True(await harness.Published.Any<PollOptionDeleted>(TestContext.Current.CancellationToken));
            Assert.True(await harness.Consumed.Any<PollOptionDeleted>(TestContext.Current.CancellationToken));

            var consumerHarness = harness.GetConsumerHarness<PollOptionDeletedConsumer>();
            Assert.True(await consumerHarness.Consumed.Any<PollOptionDeleted>(TestContext.Current.CancellationToken));

            pollOptionRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOptionCache, object>>>()), Times.Once);
            voteRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<Expression<Func<Vote, bool>>>()), Times.Once);
            voteRepositoryMock.Verify(r => r.DeleteRangeAndSaveAsync(It.IsAny<Vote[]>()), Times.Once);
            pollOptionRepositoryMock.Verify(r => r.DeleteAndSaveAsync(It.IsAny<PollOptionCache>()), Times.Once);
        }
        finally
        {
            await harness.Stop(TestContext.Current.CancellationToken);
        }
    }
    [Fact]
    public async Task PollOptionDeletedConsumer_WhenPollOptionNotFound_ReturnsFault()
    {
        // Arrange
        var pollOptionRepositoryMock = new Mock<IRepository<PollOptionCache, Guid>>();
        var voteRepositoryMock = new Mock<IRepository<Vote, Guid>>();

        await using var provider = new ServiceCollection()
            .AddSingleton(pollOptionRepositoryMock.Object)
            .AddSingleton(voteRepositoryMock.Object)
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<PollOptionDeletedConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        try
        {
            var bus = provider.GetRequiredService<IBus>();
            var poll = VoteDataFactory.CreatePoll();

            pollOptionRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOptionCache, object>>>()))
                .ReturnsAsync((PollOptionCache?)null);

            // Act
            await bus.Publish(new PollOptionDeleted(poll.PollOptions.First().Id), TestContext.Current.CancellationToken);

            // Assert
            Assert.True(await harness.Published.Any<Fault<PollOptionDeleted>>(TestContext.Current.CancellationToken));
        }
        finally
        {
            await harness.Stop(TestContext.Current.CancellationToken);
        }
    }
}
