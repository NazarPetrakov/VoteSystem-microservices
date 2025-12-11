using Common.Contracts.Vote;
using MassTransit;
using VoteService.API.Contracts.Dtos;
using VoteService.API.Interfaces;

namespace VoteService.API.Publishers;

public class VotePublisher(IPublishEndpoint publishEndpoint, ILogger<VotePublisher> logger) : IVotePublisher
{
    public async Task NotifyVoteCreatedAsync(VoteResponse voteResponse, CancellationToken cancellationToken)
    {
        if (!voteResponse.PollId.HasValue || !voteResponse.PollOptionId.HasValue)
        {
            logger.LogError($"{typeof(VotePublisher).GetMethod("NotifyVoteCreatedAsync")!}:" +
                "PollId or PollOptionId is null. VoteId: {voteResponse.VoteId}");
            return;
        }

        await publishEndpoint.Publish(new VoteCreated(voteResponse.VoteId,
            voteResponse.PollId.Value, voteResponse.PollOptionId.Value),
                cancellationToken);
    }

    public async Task NotifyVoteDeletedAsync(VoteResponse voteResponse, CancellationToken cancellationToken)
    {
        if (!voteResponse.PollId.HasValue || !voteResponse.PollOptionId.HasValue)
        {
            logger.LogError($"{typeof(VotePublisher).GetMethod("NotifyVoteDeletedAsync")!}:" +
                "PollId or PollOptionId is null. VoteId: {voteResponse.VoteId}");
            return;
        }

        await publishEndpoint.Publish(new VoteDeleted(voteResponse.VoteId,
            voteResponse.PollId.Value, voteResponse.PollOptionId.Value),
                cancellationToken);
    }
}
