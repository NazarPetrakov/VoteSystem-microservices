using System.Linq.Expressions;
using Common.Repositories;
using Moq;
using PollService.API.Extensions;
using PollService.API.Helpers;
using PollService.API.Interfaces;
using PollService.API.Models;
using PollService.API.Orchestrators;
using PollService.Tests.Factories;

namespace PollService.Tests;

public class PollOrchestratorTests
{
    private readonly Mock<IRepository<Poll, Guid>> _pollRepositoryMock;
    private readonly Mock<IRepository<PollOption, Guid>> _pollOptionRepositoryMock;
    private readonly Mock<IPollPublisher> _pollPublisherMock;
    private readonly PollOrchestrator _orchestrator;

    public PollOrchestratorTests()
    {
        _pollRepositoryMock = new Mock<IRepository<Poll, Guid>>();
        _pollOptionRepositoryMock = new Mock<IRepository<PollOption, Guid>>();
        _pollPublisherMock = new Mock<IPollPublisher>();

        _orchestrator = new PollOrchestrator(
            _pollRepositoryMock.Object,
            _pollOptionRepositoryMock.Object,
            _pollPublisherMock.Object
        );
    }
    [Fact]
    public async Task GetAllAsync_ReturnsThreePolls()
    {
        // Arrange
        var polls = PollDataFactory.CreatePolls(3);

        _pollRepositoryMock.Setup(r => r.GetAllAsync(null)).ReturnsAsync(polls);

        // Act
        var result = await _orchestrator.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Data!.Count);
        Assert.Equal("poll 1", result.Data[0].Title);
        Assert.Equal("poll 2", result.Data[1].Title);
        Assert.Equal("poll 3", result.Data[2].Title);

        _pollRepositoryMock.Verify(r => r.GetAllAsync(null), Times.Once);
    }
    [Fact]
    public async Task GetByIdAsync_ReturnsPollWithId()
    {
        // Arrange
        var poll = PollDataFactory.CreatePoll(false);

        _pollRepositoryMock.Setup(r => r.GetAsync(poll.Id, p => p.PollOptions)).ReturnsAsync(poll);

        // Act
        var result = await _orchestrator.GetAsync(poll.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("poll", result.Data?.Title);

        _pollRepositoryMock.Verify(r => r.GetAsync(poll.Id, p => p.PollOptions), Times.Once);
    }
    [Fact]
    public async Task GetByIdAsync_WhenPollNotFound_ReturnsFailure()
    {
        // Arrange
        var pollId = Guid.NewGuid();

        _pollRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), p => p.PollOptions)).ReturnsAsync((Poll?)null);

        // Act
        var result = await _orchestrator.GetAsync(pollId);

        // Assert
        Assert.True(!result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal("Polls.NotFound", result.Error.Code);

        _pollRepositoryMock.Verify(r => r.GetAsync(pollId, p => p.PollOptions), Times.Once);
    }
    [Fact]
    public async Task CreateAsync_WithCreatePollRequest_ReturnsSuccessWithCreatedPoll()
    {
        // Arrange
        var createPollRequest = PollDataFactory.GetCreatePollRequest();

        _pollRepositoryMock.Setup(r => r.CreateAndSaveAsync(It.IsAny<Poll>()))
            .ReturnsAsync(createPollRequest.ToEntity());

        _pollOptionRepositoryMock.Setup(r => r.CreateAndSaveAsync(It.IsAny<PollOption>()))
            .ReturnsAsync(PollDataFactory.CreatePollOption());

        _pollPublisherMock.Setup(p => p.NotifyPollCreatedAsync(It.IsAny<PollResponse>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _orchestrator.CreateAsync(createPollRequest, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, createPollRequest.pollOptionsToCreate?.Count);
        Assert.Equal("create poll", result.Data?.Title);

        _pollRepositoryMock.Verify(r => r.CreateAndSaveAsync(It.IsAny<Poll>()), Times.Once);

        _pollOptionRepositoryMock.Verify(r => r.CreateAndSaveAsync(It.Is<PollOption>(o => o.Text == "option 1" && o.Order == 1)), Times.Once);
        _pollOptionRepositoryMock.Verify(r => r.CreateAndSaveAsync(It.Is<PollOption>(o => o.Text == "option 2" && o.Order == 2)), Times.Once);

        _pollPublisherMock.Verify(p => p.NotifyPollCreatedAsync(It.IsAny<PollResponse>(), CancellationToken.None), Times.Once);
    }
    [Fact]
    public async Task DeleteAsync_SuccessDeleteAndNotify()
    {
        // Arrange
        var poll = PollDataFactory.CreatePoll();

        _pollRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Poll, object>>>()))
            .ReturnsAsync(poll);
        _pollRepositoryMock.Setup(r => r.DeleteAndSaveAsync(It.IsAny<Poll>()))
            .Returns(Task.CompletedTask);
        _pollPublisherMock.Setup(p => p.NotifyPollDeletedAsync(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _orchestrator.DeleteAsync(poll.Id, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        _pollRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Poll, object>>>()), Times.Once);
        _pollRepositoryMock.Verify(r => r.DeleteAndSaveAsync(It.IsAny<Poll>()), Times.Once);

        _pollPublisherMock.Verify(p => p.NotifyPollDeletedAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Once);
    }
    [Fact]
    public async Task DeleteAsync_WhenPollNotFound_ReturnsFailure()
    {
        // Arrange
        _pollRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Poll, object>>>()))
            .ReturnsAsync((Poll?)null);

        // Act
        var result = await _orchestrator.DeleteAsync(It.IsAny<Guid>(), CancellationToken.None);

        // Assert
        Assert.True(!result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("Polls.NotFound", result.Error.Code);

        _pollRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Poll, object>>>()), Times.Once);
        _pollRepositoryMock.Verify(r => r.DeleteAndSaveAsync(It.IsAny<Poll>()), Times.Never);

        _pollPublisherMock.Verify(p => p.NotifyPollDeletedAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Never);
    }
    [Fact]
    public async Task UpdateAsync_SuccessUpdate()
    {
        // Arrange
        var updatePollRequest = PollDataFactory.GetUpdatePollRequest();
        var poll = PollDataFactory.CreatePoll();
        var pollEntity = updatePollRequest.ToEntity(poll);

        _pollRepositoryMock.Setup(r => r.GetAsync(updatePollRequest.Id, It.IsAny<Expression<Func<Poll, object>>>()))
            .ReturnsAsync(poll);
        _pollRepositoryMock.Setup(r => r.UpdateAndSaveAsync(pollEntity))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _orchestrator.UpdateAsync(updatePollRequest);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(pollEntity.UpdatedAt);

        _pollRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Poll, object>>>()), Times.Once);
        _pollRepositoryMock.Verify(r => r.UpdateAndSaveAsync(It.IsAny<Poll>()), Times.Once);
    }
    [Fact]
    public async Task UpdateAsync_WhenPollNotFound_ReturnsFailure()
    {
        // Arrange
        _pollRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Poll, object>>>()))
            .ReturnsAsync((Poll?)null);

        // Act
        var result = await _orchestrator.UpdateAsync(PollDataFactory.GetUpdatePollRequest());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("Polls.NotFound", result.Error.Code);

        _pollRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Poll, object>>>()), Times.Once);
        _pollRepositoryMock.Verify(r => r.UpdateAndSaveAsync(It.IsAny<Poll>()), Times.Never);
    }
}
