using Common.Result;
using VoteService.API.Contracts.Dtos;

namespace VoteService.API.Interfaces;

public interface IVoteOrchestrator
{
    Task<Result<List<VoteResponse>>> GetAllAsync();
    Task<Result<VoteResponse>> GetAsync(Guid voteId);
    Task<Result<VoteResponse>> CreateAsync(CreateVoteRequest createVoteRequest);
    Task<Result> DeleteAsync(Guid voteId);
}
