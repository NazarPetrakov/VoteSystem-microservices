using System.Linq.Expressions;
using Common.Errors;
using Common.Repositories;
using Common.Result;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using VoteService.API.Contracts.Dtos;
using VoteService.API.Extensions;
using VoteService.API.Interfaces;
using VoteService.API.Models;

namespace VoteService.API.Orchestrators;

public class VoteOrchestrator(IRepository<Vote, Guid> voteRepository,
    IRepository<PollCache, Guid> pollRepository,
    IRepository<UserCache, int> userRepository,
    IRepository<PollOptionCache, Guid> pollOptionRepository) : IVoteOrchestrator
{
    public async Task<Result<List<VoteResponse>>> GetAllAsync(
        Expression<Func<Vote, bool>>? filter = null)
    {
        var votesQuery = voteRepository.GetAllQuery();

        if (filter is not null)
        {
            votesQuery = votesQuery.Where(filter);
        }

        var votes = await votesQuery.ToListAsync();

        return Result.Success(votes.Select(v => v.ToDto()).ToList());
    }
    public async Task<Result<VoteResponse>> GetAsync(Guid voteId)
    {
        var vote = await voteRepository.GetAsync(voteId);

        if (vote == null)
            return Result.Failure<VoteResponse>(VoteErrors.NotFound(voteId));

        return Result.Success(vote.ToDto());
    }
    public async Task<Result<VoteResponse>> CreateAsync(CreateVoteRequest createVoteRequest)
    {
        var user = await userRepository.GetAsync(createVoteRequest.UserId);
        if (user == null)
            return Result.Failure<VoteResponse>(AuthErrors.UserNotFound);

        var poll = await pollRepository.GetAsync(createVoteRequest.PollId);
        var pollOption = await pollOptionRepository.GetAsync(createVoteRequest.PollOptionId);

        if (poll == null)
            return Result.Failure<VoteResponse>(PollErrors.NotFound(createVoteRequest.PollId));
        if (pollOption == null)
            return Result.Failure<VoteResponse>(PollOptionErrors.NotFound(createVoteRequest.PollOptionId));

        var pollOptionsIds = await GetPollOptionIdsByPollAsync(poll.Id);

        if (!pollOptionsIds.Contains(pollOption.Id))
            return Result.Failure<VoteResponse>(PollErrors.MissingOption(poll.Id, pollOption.Id));

        if (poll.IsClosed)
            return Result.Failure<VoteResponse>(VoteErrors.ClosedPoll(poll.Id));

        var vote = createVoteRequest.ToEntity();

        var createdVote = await voteRepository.CreateAndSaveAsync(vote);

        return Result.Success(createdVote.ToDto());
    }
    public async Task<Result> DeleteAsync(Guid voteId)
    {
        var vote = await voteRepository.GetAsync(voteId);

        if (vote == null)
            return Result.Failure(VoteErrors.NotFound(voteId));

        await voteRepository.DeleteAndSaveAsync(vote);

        return Result.Success();
    }
    private async Task<List<Guid>> GetPollOptionIdsByPollAsync(Guid pollId)
    {
        var pollOptionsQuery = pollOptionRepository.GetAllQuery();
        return await pollOptionsQuery.Where(po => po.PollId == pollId).Select(po => po.Id)
            .ToListAsync();
    }
}
