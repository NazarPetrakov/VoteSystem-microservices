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

public class PollOptionOrchestratorTests
{
    private readonly Mock<IRepository<Poll, Guid>> _pollRepositoryMock;
    private readonly Mock<IRepository<PollOption, Guid>> _pollOptionRepositoryMock;
    private readonly Mock<IPollPublisher> _pollPublisherMock;
    private readonly PollOptionOrchestrator _orchestrator;

    public PollOptionOrchestratorTests()
    {
        _pollRepositoryMock = new Mock<IRepository<Poll, Guid>>();
        _pollOptionRepositoryMock = new Mock<IRepository<PollOption, Guid>>();
        _pollPublisherMock = new Mock<IPollPublisher>();

        _orchestrator = new PollOptionOrchestrator(
            _pollOptionRepositoryMock.Object,
            _pollRepositoryMock.Object,
            _pollPublisherMock.Object
        );
    }

    [Fact]
    public async Task GetAllAsync_ReturnsThreePollOptions()
    {
        // Arrange
        var pollOptions = PollDataFactory.CreatePollOptions(3);

        _pollOptionRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<PollOption, bool>>>()))
            .ReturnsAsync(pollOptions);

        // Act
        var result = await _orchestrator.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Data!.Count);

        Assert.Equal("option 1", result.Data[0].Text);
        Assert.Equal(1, result.Data[0].Order);

        Assert.Equal("option 2", result.Data[1].Text);
        Assert.Equal(2, result.Data[1].Order);

        _pollOptionRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<Expression<Func<PollOption, bool>>>()), Times.Once);
    }
    [Fact]
    public async Task GetByIdAsync_ReturnsPollOptionWithId()
    {
        // Arrange
        var pollOption = PollDataFactory.CreatePollOption(Guid.NewGuid());

        _pollOptionRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOption, object>>>()))
            .ReturnsAsync(pollOption);

        // Act
        var result = await _orchestrator.GetAsync(pollOption.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("pollOption", result.Data?.Text);

        _pollOptionRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOption, object>>>()), Times.Once);
    }
    [Fact]
    public async Task GetByIdAsync_WhenPollOptionNotFound_ReturnsFailure()
    {
        // Arrange
        var pollOptionId = Guid.NewGuid();

        _pollOptionRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOption, object>>>()))
            .ReturnsAsync((PollOption?)null);

        // Act
        var result = await _orchestrator.GetAsync(pollOptionId);

        // Assert
        Assert.True(!result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal("PollOptions.NotFound", result.Error.Code);

        _pollOptionRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOption, object>>>()), Times.Once);
    }
    [Fact]
    public async Task CreateAsync_WithCreatePollOptionRequest_ReturnsSuccessWithCreatedPollOption()
    {
        // Arrange
        var poll = PollDataFactory.CreatePoll();
        var createPollOptionRequest = PollDataFactory.GetCreatePollOptionRequest(poll.Id);

        _pollRepositoryMock.Setup(r => r.GetAsync(createPollOptionRequest.PollId, It.IsAny<Expression<Func<Poll, object>>>()))
            .ReturnsAsync(poll);

        _pollOptionRepositoryMock.Setup(r => r.CreateAndSaveAsync(It.IsAny<PollOption>()))
            .ReturnsAsync(createPollOptionRequest.ToEntity());

        _pollPublisherMock.Setup(p => p.NotifyPollOptionCreatedAsync(It.IsAny<PollOptionResponse>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _orchestrator.CreateAsync(createPollOptionRequest, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("poll option", result.Data?.Text);
        Assert.Equal(3, result.Data?.Order);

        _pollRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Poll, object>>>()), Times.Once);

        _pollOptionRepositoryMock.Verify(r => r.CreateAndSaveAsync(It.IsAny<PollOption>()), Times.Once);

        _pollPublisherMock.Verify(p => p.NotifyPollOptionCreatedAsync(It.IsAny<PollOptionResponse>(), CancellationToken.None), Times.Once);
    }
    [Fact]
    public async Task CreateAsync_WhenPollNotFound_ReturnsFailure()
    {
        // Arrange
        _pollRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Poll, object>>>()))
            .ReturnsAsync((Poll?)null);

        // Act
        var result = await _orchestrator.CreateAsync(PollDataFactory.GetCreatePollOptionRequest(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.True(!result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal("Polls.NotFound", result.Error.Code);

        _pollRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Poll, object>>>()), Times.Once);
    }
    [Fact]
    public async Task DeleteAsync_SuccessDeleteAndNotify()
    {
        // Arrange
        var pollOption = PollDataFactory.CreatePollOption();

        _pollOptionRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOption, object>>>()))
            .ReturnsAsync(pollOption);
        _pollOptionRepositoryMock.Setup(r => r.DeleteAndSaveAsync(It.IsAny<PollOption>()))
            .Returns(Task.CompletedTask);
        _pollPublisherMock.Setup(p => p.NotifyPollOptionDeletedAsync(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _orchestrator.DeleteAsync(pollOption.Id, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        _pollOptionRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOption, object>>>()), Times.Once);
        _pollOptionRepositoryMock.Verify(r => r.DeleteAndSaveAsync(It.IsAny<PollOption>()), Times.Once);

        _pollPublisherMock.Verify(p => p.NotifyPollOptionDeletedAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Once);
    }
    [Fact]
    public async Task DeleteAsync_WhenPollOptionNotFound_ReturnsFailure()
    {
        // Arrange
        _pollOptionRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOption, object>>>()))
            .ReturnsAsync((PollOption?)null);

        // Act
        var result = await _orchestrator.DeleteAsync(It.IsAny<Guid>(), CancellationToken.None);

        // Assert
        Assert.True(!result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("PollOptions.NotFound", result.Error.Code);

        _pollOptionRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOption, object>>>()), Times.Once);
        _pollOptionRepositoryMock.Verify(r => r.DeleteAndSaveAsync(It.IsAny<PollOption>()), Times.Never);

        _pollPublisherMock.Verify(p => p.NotifyPollOptionDeletedAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Never);
    }
    [Fact]
    public async Task UpdateAsync_SuccessUpdate()
    {
        // Arrange
        var updatePollOptionRequest = PollDataFactory.GetUpdatePollOptionRequest();
        var pollOption = PollDataFactory.CreatePollOption();
        var pollOptionEntity = updatePollOptionRequest.ToEntity(pollOption);

        _pollOptionRepositoryMock.Setup(r => r.GetAsync(updatePollOptionRequest.Id, It.IsAny<Expression<Func<PollOption, object>>>()))
            .ReturnsAsync(pollOption);
        _pollOptionRepositoryMock.Setup(r => r.UpdateAndSaveAsync(pollOptionEntity))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _orchestrator.UpdateAsync(updatePollOptionRequest);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(pollOptionEntity.UpdatedAt);

        _pollOptionRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOption, object>>>()), Times.Once);
        _pollOptionRepositoryMock.Verify(r => r.UpdateAndSaveAsync(It.IsAny<PollOption>()), Times.Once);
    }
    [Fact]
    public async Task UpdateAsync_WhenPollOptionNotFound_ReturnsFailure()
    {
        // Arrange
        _pollOptionRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOption, object>>>()))
            .ReturnsAsync((PollOption?)null);

        // Act
        var result = await _orchestrator.UpdateAsync(PollDataFactory.GetUpdatePollOptionRequest());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("PollOptions.NotFound", result.Error.Code);

        _pollOptionRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOption, object>>>()), Times.Once);
        _pollOptionRepositoryMock.Verify(r => r.UpdateAndSaveAsync(It.IsAny<PollOption>()), Times.Never);
    }
}
