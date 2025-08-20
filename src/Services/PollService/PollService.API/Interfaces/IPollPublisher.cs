using PollService.API.Helpers;

namespace PollService.API.Interfaces;

public interface IPollPublisher
{
    Task NotifyPollOptionCreatedAsync(PollOptionResponse pollOptionResponse,
        CancellationToken cancellationToken);
    Task NotifyPollCreatedAsync(PollResponse pollResponse,
        CancellationToken cancellationToken);
    Task NotifyPollOptionDeletedAsync(Guid pollOptionId,
        CancellationToken cancellationToken);
    Task NotifyPollDeletedAsync(Guid pollId,
        CancellationToken cancellationToken);
}
