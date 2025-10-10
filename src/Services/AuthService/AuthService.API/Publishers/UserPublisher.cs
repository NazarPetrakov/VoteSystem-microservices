using AuthService.API.Interfaces;
using AuthService.API.Models;
using Common.Contracts.User;
using MassTransit;

namespace AuthService.API.Publishers;

public class UserPublisher(IPublishEndpoint publishEndpoint, ILogger<UserPublisher> logger) : IUserPublisher
{
    public async Task NotifyUserCreatedAsync(AppUser user, CancellationToken cancellationToken)
    {
        try
        {
            await publishEndpoint.Publish(new UserCreated(user.Id, user.UserName!), cancellationToken);
            logger.LogInformation("Published UserCreated event for user {UserName} ({UserId})", user.UserName, user.Id);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Something went wrong while publishing UserCreated event for user {UserId}", user.Id);

        }
    }
}
