using Common.Pagination;
using Common.Result;
using PollService.API.Helpers;

namespace PollService.API.Interfaces;

public interface IPollOrchestrator
{
    Task<Result<PagedList<PollResponse>>> GetAllPagedAsync(PollQueryParams pollParams);
    Task<Result<List<PollResponse>>> GetAllAsync();
    Task<Result<PollResponse>> GetAsync(Guid id);
    Task<Result<PollResponse>> CreateAsync(CreatePollRequest createPollRequest, int userId, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(UpdatePollRequest updatePollRequest, int userId);
    Task<Result> DeleteAsync(Guid id, int userId, CancellationToken cancellationToken);
    Task<Result> ClosePollAsync(Guid pollId, int userId);
}
