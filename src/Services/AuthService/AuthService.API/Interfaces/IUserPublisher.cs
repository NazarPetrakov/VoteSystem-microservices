using AuthService.API.Models;

namespace AuthService.API.Interfaces;

public interface IUserPublisher
{
    Task NotifyUserCreatedAsync(AppUser user, CancellationToken cancellationToken);
}
