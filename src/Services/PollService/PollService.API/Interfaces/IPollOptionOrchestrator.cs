using PollService.API.Helpers;
using PollService.API.Helpers.ResultPattern;

namespace PollService.API.Interfaces;

public interface IPollOptionOrchestrator
{
    Task<Result<List<PollOptionResponse>>> GetAllAsync();
    Task<Result<PollOptionResponse>> GetAsync(Guid id);
    Task<Result<PollOptionResponse>> CreateAsync(CreatePollOptionRequest createPollOptionRequest);
    Task<Result> UpdateAsync(UpdatePollOptionRequest updatePollOptionRequest);
    Task<Result> DeleteAsync(Guid id);
}
