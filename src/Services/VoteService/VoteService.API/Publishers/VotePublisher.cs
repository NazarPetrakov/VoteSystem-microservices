using Common.Contracts.Vote;
using MassTransit;
using VoteService.API.Contracts.Dtos;
using VoteService.API.Interfaces;

namespace VoteService.API.Publishers;

public class VotePublisher(IPublishEndpoint publishEndpoint, ILogger<VotePublisher> _logger) : IVotePublisher
{
    public async Task NotifyVoteCreatedAsync(VoteResponse voteResponse, CancellationToken cancellationToken)
    {
        if (!voteResponse.PollId.HasValue || !voteResponse.PollOptionId.HasValue)
        {
            _logger.LogError("Vote with ID {VoteId} was not created, PollId or PollOptionId is null.", voteResponse.VoteId);
            return;
        }

        await publishEndpoint.Publish(new VoteCreated(voteResponse.VoteId, voteResponse.UserId,
            voteResponse.PollId.Value, voteResponse.PollOptionId.Value),
                cancellationToken);
    }

    public async Task NotifyVoteDeletedAsync(VoteResponse voteResponse, CancellationToken cancellationToken)
    {
        if (!voteResponse.PollId.HasValue || !voteResponse.PollOptionId.HasValue)
        {
            _logger.LogError("Vote with ID {VoteId} was not created, PollId or PollOptionId is null.", voteResponse.VoteId);
            return;
        }

        await publishEndpoint.Publish(new VoteDeleted(voteResponse.VoteId, voteResponse.UserId,
            voteResponse.PollId.Value, voteResponse.PollOptionId.Value),
                cancellationToken);
    }
}
