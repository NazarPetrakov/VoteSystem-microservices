using Common.Errors;
using Common.Repositories;
using Common.Result;
using PollService.API.Extensions;
using PollService.API.Helpers;
using PollService.API.Interfaces;
using PollService.API.Models;

namespace PollService.API.Orchestrators;

public class PollOrchestrator(IRepository<Poll, Guid> pollRepository,
    IRepository<PollOption, Guid> pollOptionRepository,
    IRepository<UserCache, int> userRepository,
    IPollPublisher pollPublisher) : IPollOrchestrator
{
    public async Task<Result<List<PollResponse>>> GetAllAsync()
    {
        var polls = await pollRepository.GetAllAsync();

        return Result.Success(polls.Select(p => p.ToDto()).ToList());
    }
    public async Task<Result<PollResponse>> GetAsync(Guid id)
    {
        var poll = await pollRepository.GetAsync(id, p => p.PollOptions, p => p.User);

        if (poll == null)
            return Result.Failure<PollResponse>(PollErrors.NotFound(id));

        return Result.Success(poll.ToDto());
    }
    public async Task<Result<PollResponse>> CreateAsync(CreatePollRequest createPollRequest, CancellationToken cancellationToken)
    {
        int order = 1;
        var poll = createPollRequest.ToEntity();

        var user = await userRepository.GetAsync(poll.UserId);
        if (user is null)
            return Result.Failure<PollResponse>(AuthErrors.UserNotFound);

        await pollRepository.CreateAndSaveAsync(poll);

        // Creating options for poll
        if (createPollRequest.pollOptionsToCreate != null &&
            createPollRequest.pollOptionsToCreate.Count != 0)
        {
            foreach (var pollOption in createPollRequest.pollOptionsToCreate)
            {
                var pollOptionToCreate = pollOption.ToEntity(poll.Id);
                pollOptionToCreate.Order = order;

                await pollOptionRepository.CreateAndSaveAsync(pollOptionToCreate);

                order++;
            }
        }

        var pollResponse = poll.ToDto();

        await pollPublisher.NotifyPollCreatedAsync(pollResponse, cancellationToken);

        return Result.Success(pollResponse);
    }
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var poll = await pollRepository.GetAsync(id);

        if (poll == null)
            return Result.Failure(PollErrors.NotFound(id));

        await pollRepository.DeleteAndSaveAsync(poll);

        await pollPublisher.NotifyPollDeletedAsync(id, cancellationToken);

        return Result.Success();
    }
    public async Task<Result> ClosePollAsync(Guid pollId)
    {
        var pollFromId = await pollRepository.GetAsync(pollId);

        if (pollFromId == null)
            return Result.Failure(PollErrors.NotFound(pollId));

        if (pollFromId.IsClosed)
            return Result.Failure(PollErrors.AlreadyClosed(pollId));

        pollFromId.IsClosed = true;

        await pollRepository.UpdateAndSaveAsync(pollFromId);

        return Result.Success();
    }
    public async Task<Result> UpdateAsync(UpdatePollRequest updatePollRequest)
    {
        var pollFromId = await pollRepository.GetAsync(updatePollRequest.Id);

        if (pollFromId == null)
            return Result.Failure<PollResponse>(PollErrors.NotFound(updatePollRequest.Id));

        var poll = updatePollRequest.ToEntity(pollFromId);

        await pollRepository.UpdateAndSaveAsync(poll);

        return Result.Success();
    }
}
