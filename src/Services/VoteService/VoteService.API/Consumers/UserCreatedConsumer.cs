using System;
using Common.Contracts.User;
using Common.Repositories;
using MassTransit;
using VoteService.API.Models;

namespace VoteService.API.Consumers;

public class UserCreatedConsumer(ILogger<UserCreatedConsumer> logger,
    IRepository<UserCache, int> userRepository) : IConsumer<UserCreated>
{
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var userCreated = context.Message;

        await userRepository.CreateAndSaveAsync(new UserCache
        {
            Id = userCreated.UserId,
            UserName = userCreated.UserName,
        });

        logger.LogInformation("User with ID {UserId} created", userCreated.UserId);
    }
}
