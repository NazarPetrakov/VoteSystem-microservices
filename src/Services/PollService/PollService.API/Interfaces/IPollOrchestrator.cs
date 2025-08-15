using Common.Result;
using PollService.API.Helpers;

namespace PollService.API.Interfaces;

public interface IPollOrchestrator
{
    Task<Result<List<PollResponse>>> GetAllAsync();
    Task<Result<PollResponse>> GetAsync(Guid id);
    Task<Result<PollResponse>> CreateAsync(CreatePollRequest createPollRequest);
    Task<Result> UpdateAsync(UpdatePollRequest updatePollRequest);
    Task<Result> DeleteAsync(Guid id);
}
