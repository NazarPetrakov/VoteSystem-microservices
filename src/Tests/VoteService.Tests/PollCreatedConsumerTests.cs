using Common.Contracts.Poll;
using Common.Repositories;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VoteService.API.Consumers;
using VoteService.API.Models;

namespace VoteService.Tests;

public class PollCreatedConsumerTests
{
    [Fact]
    public async Task ShouldConsume_PollCreated_AndSaveToRepository()
    {
        // Arrange
        var pollRepositoryMock = new Mock<IRepository<PollCache, Guid>>();

        await using var provider = new ServiceCollection()
            .AddSingleton(pollRepositoryMock.Object)
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<PollCreatedConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        try
        {
            var bus = provider.GetRequiredService<IBus>();

            var pollId = Guid.NewGuid();

            // Act
            await bus.Publish(new PollCreated(pollId, false), TestContext.Current.CancellationToken);

            // Assert
            Assert.True(await harness.Published.Any<PollCreated>(TestContext.Current.CancellationToken));
            Assert.True(await harness.Consumed.Any<PollCreated>(TestContext.Current.CancellationToken));

            var consumerHarness = harness.GetConsumerHarness<PollCreatedConsumer>();
            Assert.True(await consumerHarness.Consumed.Any<PollCreated>(TestContext.Current.CancellationToken));

            pollRepositoryMock.Verify(r => r.CreateAndSaveAsync(
                It.Is<PollCache>(p => p.Id == pollId && p.IsClosed == false)),
                Times.Once);
        }
        finally
        {
            await harness.Stop(TestContext.Current.CancellationToken);
        }
    }
}
