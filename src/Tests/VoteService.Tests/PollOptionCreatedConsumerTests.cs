using System.Linq.Expressions;
using Common.Contracts.PollOption;
using Common.Repositories;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VoteService.API.Consumers;
using VoteService.API.Exceptions;
using VoteService.API.Models;
using VoteService.Tests.Factories;

namespace VoteService.Tests;

public class PollOptionCreatedConsumerTests
{
    [Fact]
    public async Task ShouldConsume_PollOptionCreated_AndSaveToRepository()
    {
        // Arrange
        var pollOptionRepositoryMock = new Mock<IRepository<PollOptionCache, Guid>>();
        var pollRepositoryMock = new Mock<IRepository<PollCache, Guid>>();

        await using var provider = new ServiceCollection()
            .AddSingleton(pollOptionRepositoryMock.Object)
            .AddSingleton(pollRepositoryMock.Object)
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<PollOptionCreatedConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        try
        {
            var bus = provider.GetRequiredService<IBus>();

            var poll = VoteDataFactory.CreatePoll();
            var pollOptionCreated = new PollOptionCreated(poll.PollOptions.First().Id, poll.Id);

            pollRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()))
                .ReturnsAsync(poll);

            // Act
            await bus.Publish(pollOptionCreated, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(await harness.Published.Any<PollOptionCreated>(TestContext.Current.CancellationToken));
            Assert.True(await harness.Consumed.Any<PollOptionCreated>(TestContext.Current.CancellationToken));

            var consumerHarness = harness.GetConsumerHarness<PollOptionCreatedConsumer>();
            Assert.True(await consumerHarness.Consumed.Any<PollOptionCreated>(TestContext.Current.CancellationToken));

            pollRepositoryMock.Verify(r => r.GetAsync(
                It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()),
                Times.Once);
            pollOptionRepositoryMock.Verify(r => r.CreateAndSaveAsync(
                It.Is<PollOptionCache>(p => p.Id == pollOptionCreated.PollOptionId && p.PollId == poll.Id)),
                Times.Once);
        }
        finally
        {
            await harness.Stop(TestContext.Current.CancellationToken);
        }
    }
    [Fact]
    public async Task PollOptionCreatedConsumer_WhenPollNotFound_ReturnsFault()
    {
        // Arrange
        var pollOptionRepositoryMock = new Mock<IRepository<PollOptionCache, Guid>>();
        var pollRepositoryMock = new Mock<IRepository<PollCache, Guid>>();

        await using var provider = new ServiceCollection()
            .AddSingleton(pollOptionRepositoryMock.Object)
            .AddSingleton(pollRepositoryMock.Object)
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<PollOptionCreatedConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        try
        {
            var bus = provider.GetRequiredService<IBus>();

            var poll = VoteDataFactory.CreatePoll();
            var pollOptionCreated = new PollOptionCreated(poll.PollOptions.First().Id, poll.Id);

            pollRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()))
                .ReturnsAsync((PollCache?)null);

            // Act
            await bus.Publish(pollOptionCreated, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(await harness.Published.Any<Fault<PollOptionCreated>>(TestContext.Current.CancellationToken));
        }
        finally
        {
            await harness.Stop(TestContext.Current.CancellationToken);
        }
    }
}
