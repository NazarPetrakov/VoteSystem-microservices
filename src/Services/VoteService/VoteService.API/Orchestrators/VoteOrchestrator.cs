using Common.Errors;
using Common.Repositories;
using Common.Result;
using MassTransit.Initializers;
using VoteService.API.Contracts.Dtos;
using VoteService.API.Extensions;
using VoteService.API.Interfaces;
using VoteService.API.Models;

namespace VoteService.API.Orchestrators;

public class VoteOrchestrator(IRepository<Vote, Guid> voteRepository,
    IRepository<PollCache, Guid> pollRepository,
    IRepository<PollOptionCache, Guid> pollOptionRepository) : IVoteOrchestrator
{
    public async Task<Result<List<VoteResponse>>> GetAllAsync()
    {
        var votes = await voteRepository.GetAllAsync();

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
        var poll = await pollRepository.GetAsync(createVoteRequest.PollId);
        var pollOption = await pollOptionRepository.GetAsync(createVoteRequest.PollOptionId);

        if (poll == null)
            return Result.Failure<VoteResponse>(PollErrors.NotFound(createVoteRequest.PollId));
        if (pollOption == null)
            return Result.Failure<VoteResponse>(PollOptionErrors.NotFound(createVoteRequest.PollOptionId));

        var pollOptions = await pollOptionRepository.GetAllAsync(po => po.PollId == poll.Id);
        var pollOptionsIds = pollOptions.Select(po => po.Id).ToList();

        if (!pollOptionsIds.Contains(pollOption.Id))
            return Result.Failure<VoteResponse>(PollErrors.MissingOption(poll.Id, pollOption.Id));

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
}
