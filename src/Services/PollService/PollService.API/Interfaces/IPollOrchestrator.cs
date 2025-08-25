using Common.Result;
using PollService.API.Helpers;

namespace PollService.API.Interfaces;

public interface IPollOrchestrator
{
    Task<Result<List<PollResponse>>> GetAllAsync();
    Task<Result<PollResponse>> GetAsync(Guid id);
    Task<Result<PollResponse>> CreateAsync(CreatePollRequest createPollRequest, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(UpdatePollRequest updatePollRequest);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
