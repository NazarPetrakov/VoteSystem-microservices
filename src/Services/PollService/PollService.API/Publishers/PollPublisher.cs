using Common.Contracts.Poll;
using Common.Contracts.PollOption;
using MassTransit;
using PollService.API.Helpers;
using PollService.API.Interfaces;

namespace PollService.API.Publishers;

public class PollPublisher(IPublishEndpoint publishEndpoint) : IPollPublisher
{
    public async Task NotifyPollOptionCreatedAsync(PollOptionResponse pollOptionResponse,
        CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(
            new PollOptionCreated(pollOptionResponse.Id, pollOptionResponse.PollId),
                cancellationToken);
    }
    public async Task NotifyPollCreatedAsync(PollResponse pollResponse,
        CancellationToken cancellationToken)
    {
        var pollId = pollResponse.Id;
        var userId = pollResponse.UserId;
        var pollOptionIds = pollResponse.pollOptions.Select(o => o.Id).ToList();

        await publishEndpoint.Publish(new PollCreated(pollId, userId, pollOptionIds),
            cancellationToken);
    }
    public async Task NotifyPollOptionDeletedAsync(Guid pollOptionId,
        CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(
            new PollOptionDeleted(pollOptionId),
                cancellationToken);
    }
    public async Task NotifyPollDeletedAsync(Guid pollId, int userId,
        CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new PollDeleted(pollId, userId),
            cancellationToken);
    }
}
