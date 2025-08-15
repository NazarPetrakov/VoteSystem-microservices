using Common.Errors;
using Common.Repositories;
using Common.Result;
using PollService.API.Extensions;
using PollService.API.Helpers;
using PollService.API.Interfaces;
using PollService.API.Models;

namespace PollService.API.Orchestrators;

public class PollOptionOrchestrator(IRepository<PollOption> pollOptionRepository,
    IRepository<Poll> pollRepository) : IPollOptionOrchestrator
{
    public async Task<Result<List<PollOptionResponse>>> GetAllAsync()
    {
        var pollOptions = await pollOptionRepository.GetAllAsync();

        return Result.Success(pollOptions.Select(p => p.ToDto()).ToList());
    }
    public async Task<Result<PollOptionResponse>> GetAsync(Guid id)
    {
        var pollOption = await pollOptionRepository.GetAsync(id);

        if (pollOption == null)
            return Result.Failure<PollOptionResponse>(PollOptionErrors.NotFound(id));

        return Result.Success(pollOption.ToDto());
    }
    public async Task<Result<PollOptionResponse>> CreateAsync(CreatePollOptionRequest createPollOptionRequest)
    {
        var poll = await pollRepository.GetAsync(createPollOptionRequest.PollId, p => p.PollOptions);

        if (poll == null)
            return Result.Failure<PollOptionResponse>(PollErrors.NotFound(createPollOptionRequest.PollId));

        var pollOption = createPollOptionRequest.ToEntity();
        pollOption.Order = poll.PollOptions.Count + 1;

        await pollOptionRepository.CreateAndSaveAsync(pollOption);

        return Result.Success(pollOption.ToDto());
    }
    public async Task<Result> DeleteAsync(Guid id)
    {
        var pollOption = await pollOptionRepository.GetAsync(id);

        if (pollOption == null)
            return Result.Failure(PollOptionErrors.NotFound(id));

        await pollOptionRepository.DeleteAndSaveAsync(pollOption);

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
