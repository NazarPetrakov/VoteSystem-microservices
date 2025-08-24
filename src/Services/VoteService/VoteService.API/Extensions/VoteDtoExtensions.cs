using VoteService.API.Contracts.Dtos;
using VoteService.API.Models;

namespace VoteService.API.Extensions;

public static class VoteDtoExtensions
{
    public static VoteResponse ToDto(this Vote vote)
    {
        return new VoteResponse(vote.Id, vote.UserId, vote.PollId, vote.PollOptionId);
    }
    public static Vote ToEntity(this CreateVoteRequest createVoteRequest)
    {
        return new Vote
        {
            UserId = createVoteRequest.UserId,
            PollId = createVoteRequest.PollId,
            PollOptionId = createVoteRequest.PollOptionId
        };
    }
}
