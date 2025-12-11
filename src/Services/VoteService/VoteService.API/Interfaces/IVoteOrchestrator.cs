using System.Linq.Expressions;
using Common.Result;
using VoteService.API.Contracts.Dtos;
using VoteService.API.Models;

namespace VoteService.API.Interfaces;

public interface IVoteOrchestrator
{
    Task<Result<List<VoteResponse>>> GetAllAsync(Expression<Func<Vote, bool>>? filter = null);
    Task<Result<VoteResponse>> GetAsync(Guid voteId);
    Task<Result<VoteResponse>> CreateAsync(CreateVoteRequest createVoteRequest, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid voteId, CancellationToken cancellationToken);
}
