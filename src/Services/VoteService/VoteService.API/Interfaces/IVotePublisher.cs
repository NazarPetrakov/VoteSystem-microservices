using VoteService.API.Contracts.Dtos;

namespace VoteService.API.Interfaces;

public interface IVotePublisher
{
    Task NotifyVoteCreatedAsync(VoteResponse voteResponse,
        CancellationToken cancellationToken);
    Task NotifyVoteDeletedAsync(VoteResponse voteResponse,
        CancellationToken cancellationToken);
}
