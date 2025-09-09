using System.Linq.Expressions;
using Common.Repositories;
using Moq;
using VoteService.API.Extensions;
using VoteService.API.Interfaces;
using VoteService.API.Models;
using VoteService.API.Orchestrators;
using VoteService.Tests.Factories;

namespace VoteService.Tests;

public class VoteOrchestratorTests
{
    private readonly Mock<IRepository<PollCache, Guid>> _pollRepositoryMock;
    private readonly Mock<IRepository<PollOptionCache, Guid>> _pollOptionRepositoryMock;
    private readonly Mock<IRepository<Vote, Guid>> _voteRepositoryMock;
    private readonly IVoteOrchestrator _voteOrchestrator;
    public VoteOrchestratorTests()
    {
        _pollRepositoryMock = new Mock<IRepository<PollCache, Guid>>();
        _pollOptionRepositoryMock = new Mock<IRepository<PollOptionCache, Guid>>();
        _voteRepositoryMock = new Mock<IRepository<Vote, Guid>>();

        _voteOrchestrator = new VoteOrchestrator(_voteRepositoryMock.Object, _pollRepositoryMock.Object, _pollOptionRepositoryMock.Object);
    }
    [Fact]
    public async Task GetAllAsync_ReturnsSuccessWithVotes()
    {
        //Arrange
        var votes = VoteDataFactory.CreateVotes();

        _voteRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
            .ReturnsAsync(votes);

        //Act
        var result = await _voteOrchestrator.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(votes[0].ToDto(), result.Data?[0]);

        _voteRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<Expression<Func<Vote, bool>>>()), Times.Once());
    }
    [Fact]
    public async Task GetAsync_WithValidId_ReturnsSuccessWithVote()
    {
        //Arrange
        var vote = VoteDataFactory.CreateVote();

        _voteRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Vote, object>>>()))
            .ReturnsAsync(vote);

        //Act
        var result = await _voteOrchestrator.GetAsync(vote.Id);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(vote.ToDto(), result.Data);

        _voteRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Vote, object>>>()), Times.Once);
    }
    [Fact]
    public async Task GetAsync_WhenVoteNotFound_ReturnsFailure()
    {
        //Arrange
        _voteRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Vote, object>>>()))
            .ReturnsAsync((Vote?)null);

        //Act
        var result = await _voteOrchestrator.GetAsync(Guid.NewGuid());

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal("Votes.NotFound", result.Error.Code);

        _voteRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Vote, object>>>()), Times.Once);
    }
    [Fact]
    public async Task CreateAsync_SuccessCreateVote()
    {
        //Arrange
        var poll = VoteDataFactory.CreatePoll();
        var pollOptions = VoteDataFactory.CreatePollOptions(poll.Id);
        var createVoteRequest = VoteDataFactory.GetCreateVoteRequest(poll.Id, pollOptions[0].Id);
        var vote = VoteDataFactory.CreateVote(poll.Id, pollOptions[0].Id);

        _pollRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()))
            .ReturnsAsync(poll);
        _pollOptionRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOptionCache, object>>>()))
            .ReturnsAsync(pollOptions[0]);
        _pollOptionRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<PollOptionCache, bool>>>()))
            .ReturnsAsync(pollOptions);
        _voteRepositoryMock.Setup(r => r.CreateAndSaveAsync(It.IsAny<Vote>()))
            .ReturnsAsync(vote);

        //Act
        var result = await _voteOrchestrator.CreateAsync(createVoteRequest);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(vote.PollId, result.Data?.PollId);
        Assert.Equal(vote.PollOptionId, result.Data?.PollOptionId);

        _pollRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()), Times.Once);
        _pollOptionRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOptionCache, object>>>()), Times.Once);
        _pollOptionRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<Expression<Func<PollOptionCache, bool>>>()), Times.Once);
        _voteRepositoryMock.Verify(r => r.CreateAndSaveAsync(It.Is<Vote>(v => v.PollId == poll.Id && v.PollOptionId == pollOptions[0].Id)), Times.Once);
    }
    [Fact]
    public async Task CreateAsync_WhenPollNotFound_ReturnsFailure()
    {
        //Arrange
        var createVoteRequest = VoteDataFactory.GetCreateVoteRequest(Guid.NewGuid(), Guid.NewGuid());
        _pollRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()))
            .ReturnsAsync((PollCache?)null);

        //Act
        var result = await _voteOrchestrator.CreateAsync(createVoteRequest);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal("Polls.NotFound", result.Error.Code);

        _pollRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()), Times.Once);
        _pollOptionRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOptionCache, object>>>()), Times.Once);
        _pollOptionRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<Expression<Func<PollOptionCache, bool>>>()), Times.Never);
        _voteRepositoryMock.Verify(r => r.CreateAndSaveAsync(It.IsAny<Vote>()), Times.Never);
    }
    [Fact]
    public async Task CreateAsync_WhenPollOptionNotFound_ReturnsFailure()
    {
        //Arrange
        var createVoteRequest = VoteDataFactory.GetCreateVoteRequest(Guid.NewGuid(), Guid.NewGuid());
        _pollRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()))
            .ReturnsAsync(VoteDataFactory.CreatePoll());
        _pollOptionRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOptionCache, object>>>()))
            .ReturnsAsync((PollOptionCache?)null);

        //Act
        var result = await _voteOrchestrator.CreateAsync(createVoteRequest);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal("PollOptions.NotFound", result.Error.Code);

        _pollRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()), Times.Once);
        _pollOptionRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOptionCache, object>>>()), Times.Once);
        _pollOptionRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<Expression<Func<PollOptionCache, bool>>>()), Times.Never);
        _voteRepositoryMock.Verify(r => r.CreateAndSaveAsync(It.IsAny<Vote>()), Times.Never);
    }
    [Fact]
    public async Task CreateAsync_WhenPollDoesNotContainPollOption_ReturnsFailure()
    {
        //Arrange
        var poll = VoteDataFactory.CreatePoll();
        var pollOption = VoteDataFactory.CreatePollOption(poll.Id);
        var pollOptions = VoteDataFactory.CreatePollOptions(Guid.NewGuid());
        var createVoteRequest = VoteDataFactory.GetCreateVoteRequest(poll.Id, pollOptions[0].Id);

        _pollRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()))
            .ReturnsAsync(poll);
        _pollOptionRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOptionCache, object>>>()))
            .ReturnsAsync(pollOption);
        _pollOptionRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<PollOptionCache, bool>>>()))
            .ReturnsAsync(pollOptions);

        //Act
        var result = await _voteOrchestrator.CreateAsync(createVoteRequest);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal("Polls.MissingOption", result.Error.Code);

        _pollRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()), Times.Once);
        _pollOptionRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOptionCache, object>>>()), Times.Once);
        _pollOptionRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<Expression<Func<PollOptionCache, bool>>>()), Times.Once);
        _voteRepositoryMock.Verify(r => r.CreateAndSaveAsync(It.IsAny<Vote>()), Times.Never);
    }
    [Fact]
    public async Task CreateAsync_WhenPollIsClosed_ReturnsFailure()
    {
        //Arrange
        var poll = VoteDataFactory.CreatePoll(isClosed: true);
        var pollOptions = VoteDataFactory.CreatePollOptions(Guid.NewGuid());
        var createVoteRequest = VoteDataFactory.GetCreateVoteRequest(poll.Id, pollOptions[0].Id);

        _pollRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()))
            .ReturnsAsync(poll);
        _pollOptionRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOptionCache, object>>>()))
            .ReturnsAsync(pollOptions[0]);
        _pollOptionRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<PollOptionCache, bool>>>()))
            .ReturnsAsync(pollOptions);

        //Act
        var result = await _voteOrchestrator.CreateAsync(createVoteRequest);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal("Votes.ClosedPoll", result.Error.Code);

        _pollRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollCache, object>>>()), Times.Once);
        _pollOptionRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<PollOptionCache, object>>>()), Times.Once);
        _pollOptionRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<Expression<Func<PollOptionCache, bool>>>()), Times.Once);
        _voteRepositoryMock.Verify(r => r.CreateAndSaveAsync(It.IsAny<Vote>()), Times.Never);
    }
    [Fact]
    public async Task DeleteAsync_ReturnsSuccess()
    {
        //Arrange
        var vote = VoteDataFactory.CreateVote();

        _voteRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Vote, object>>>()))
            .ReturnsAsync(vote);
        _voteRepositoryMock.Setup(r => r.DeleteAndSaveAsync(It.IsAny<Vote>()))
            .Returns(Task.CompletedTask);

        //Act
        var result = await _voteOrchestrator.DeleteAsync(vote.Id);

        //Assert
        Assert.True(result.IsSuccess);

        _voteRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Vote, object>>>()), Times.Once);
        _voteRepositoryMock.Verify(r => r.DeleteAndSaveAsync(It.IsAny<Vote>()), Times.Once);
    }
    [Fact]
    public async Task DeleteAsync_WhenVoteNotFound_ReturnsFailure()
    {
        //Arrange

        _voteRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Vote, object>>>()))
            .ReturnsAsync((Vote?)null);

        //Act
        var result = await _voteOrchestrator.DeleteAsync(It.IsAny<Guid>());

        //Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("Votes.NotFound", result.Error.Code);

        _voteRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Vote, object>>>()), Times.Once);
        _voteRepositoryMock.Verify(r => r.DeleteAndSaveAsync(It.IsAny<Vote>()), Times.Never);
    }
}
