using Common.Result;
using PollService.API.Helpers;

namespace PollService.API.Interfaces;

public interface IPollOptionOrchestrator
{
    Task<Result<List<PollOptionResponse>>> GetAllAsync();
    Task<Result<PollOptionResponse>> GetAsync(Guid id);
    Task<Result<PollOptionResponse>> CreateAsync(CreatePollOptionRequest createPollOptionRequest, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(UpdatePollOptionRequest updatePollOptionRequest);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
