using Common.Errors;
using Common.Pagination;
using Common.Repositories;
using Common.Result;
using Microsoft.EntityFrameworkCore;
using PollService.API.Extensions;
using PollService.API.Helpers;
using PollService.API.Interfaces;
using PollService.API.Models;

namespace PollService.API.Orchestrators;

public class PollOptionOrchestrator(IRepository<PollOption, Guid> pollOptionRepository,
    IRepository<Poll, Guid> pollRepository,
    IPollPublisher pollPublisher) : IPollOptionOrchestrator
{
    public async Task<Result<PagedList<PollOptionResponse>>> GetAllPagedAsync(PaginationParams paginationParams)
    {
        var pollOptionsQuery = pollOptionRepository.GetAllQuery();

        var pollOptions = await pollOptionRepository.GetAllPagedAsync(pollOptionsQuery, paginationParams);

        var pollOptionResponses = pollOptions.Select(p => p.ToDto());

        var pagedPollOptionResponses = new PagedList<PollOptionResponse>(pollOptionResponses,
            pollOptions.TotalCount, paginationParams.PageNumber, paginationParams.PageSize);

        return Result.Success(pagedPollOptionResponses);
    }
    public async Task<Result<List<PollOptionResponse>>> GetAllAsync()
    {
        var pollOptionsQuery = pollOptionRepository.GetAllQuery();

        var pollOptions = await pollOptionsQuery.ToListAsync();

        return Result.Success(pollOptions.Select(p => p.ToDto()).ToList());
    }
    public async Task<Result<PollOptionResponse>> GetAsync(Guid id)
    {
        var pollOption = await pollOptionRepository.GetAsync(id);

        if (pollOption == null)
            return Result.Failure<PollOptionResponse>(PollOptionErrors.NotFound(id));

        return Result.Success(pollOption.ToDto());
    }
    public async Task<Result<PollOptionResponse>> CreateAsync(CreatePollOptionRequest createPollOptionRequest,
        CancellationToken cancellationToken)
    {
        var poll = await pollRepository.GetAsync(createPollOptionRequest.PollId, p => p.PollOptions);

        if (poll == null)
            return Result.Failure<PollOptionResponse>(PollErrors.NotFound(createPollOptionRequest.PollId));

        var pollOption = createPollOptionRequest.ToEntity();
        pollOption.Order = poll.PollOptions.Count + 1;

        await pollOptionRepository.CreateAndSaveAsync(pollOption);

        var pollOptionResponse = pollOption.ToDto();

        await pollPublisher.NotifyPollOptionCreatedAsync(pollOptionResponse, cancellationToken);

        return Result.Success(pollOption.ToDto());
    }
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var pollOption = await pollOptionRepository.GetAsync(id);

        if (pollOption == null)
            return Result.Failure(PollOptionErrors.NotFound(id));

        await pollOptionRepository.DeleteAndSaveAsync(pollOption);

        await pollPublisher.NotifyPollOptionDeletedAsync(id, cancellationToken);

        return Result.Success();
    }
    public async Task<Result> UpdateAsync(UpdatePollOptionRequest updatePollOptionRequest)
    {
        var pollOptionFromId = await pollOptionRepository.GetAsync(updatePollOptionRequest.Id);

        if (pollOptionFromId == null)
            return Result.Failure<PollOptionResponse>(PollOptionErrors.NotFound(updatePollOptionRequest.Id));

        var poll = updatePollOptionRequest.ToEntity(pollOptionFromId);

        await pollOptionRepository.UpdateAndSaveAsync(poll);

        return Result.Success();
    }
}
