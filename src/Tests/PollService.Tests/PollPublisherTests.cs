using Common.Contracts.Poll;
using Common.Contracts.PollOption;
using MassTransit;
using Moq;
using PollService.API.Extensions;
using PollService.API.Publishers;
using PollService.Tests.Factories;

namespace PollService.Tests;

public class PollPublisherTests
{
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly PollPublisher _pollPublisher;
    public PollPublisherTests()
    {
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _pollPublisher = new PollPublisher(_publishEndpointMock.Object);
    }
    [Fact]
    public async Task NotifyPollOptionCreatedAsync()
    {
        //Arrange
        var pollOption = PollDataFactory.CreatePollOption();

        //Act
        await _pollPublisher.NotifyPollOptionCreatedAsync(pollOption.ToDto(), CancellationToken.None);

        //Assert
        _publishEndpointMock.Verify(p => p.Publish(It.Is<PollOptionCreated>(msg =>
            msg.PollOptionId == pollOption.Id &&
            msg.PollId == pollOption.PollId),
                It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task NotifyPollOptionDeletedAsync()
    {
        //Arrange
        var pollOption = PollDataFactory.CreatePollOption();

        //Act
        await _pollPublisher.NotifyPollOptionDeletedAsync(pollOption.Id, CancellationToken.None);

        //Assert
        _publishEndpointMock.Verify(p => p.Publish(It.Is<PollOptionDeleted>(msg =>
            msg.PollOptionId == pollOption.Id),
                It.IsAny<CancellationToken>()));
    }
    [Fact]
    public async Task NotifyPollCreatedAsync()
    {
        //Arrange
        var poll = PollDataFactory.CreatePoll(false, 3);

        //Act
        await _pollPublisher.NotifyPollCreatedAsync(poll.ToDto(), CancellationToken.None);

        //Assert
        _publishEndpointMock.Verify(p => p.Publish(It.Is<PollCreated>(msg =>
            msg.PollId == poll.Id &&
            msg.IsClosed == poll.IsClosed),
                It.IsAny<CancellationToken>()), Times.Once);

        _publishEndpointMock.Verify(p => p.Publish(It.Is<PollOptionCreated>(msg =>
            msg.PollId == poll.Id),
                It.IsAny<CancellationToken>()), Times.Exactly(3));
    }
    [Fact]
    public async Task NotifyPollDeletedAsync()
    {
        //Arrange
        var poll = PollDataFactory.CreatePoll(false, 3);

        //Act
        await _pollPublisher.NotifyPollDeletedAsync(poll.Id, CancellationToken.None);

        //Assert
        _publishEndpointMock.Verify(p => p.Publish(It.Is<PollDeleted>(msg =>
            msg.PollId == poll.Id),
                It.IsAny<CancellationToken>()), Times.Once);
    }
}
