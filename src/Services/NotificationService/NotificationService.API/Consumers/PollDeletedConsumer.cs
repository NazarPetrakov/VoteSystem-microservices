using Common.Contracts.Poll;
using Common.Repositories;
using MassTransit;
using NotificationService.API.Exceptions;
using NotificationService.API.Models;
using NotificationService.API.Repositories;

namespace NotificationService.API.Consumers;

public class PollDeletedConsumer(ILogger<PollDeletedConsumer> logger,
    IRepository<PollWithVotes, Guid> repository,
    IUserStatsRepository userStatsRepository) : IConsumer<PollDeleted>
{
    public async Task Consume(ConsumeContext<PollDeleted> context)
    {
        var pollDeleted = context.Message;

        var poll = await repository.GetAsync(pollDeleted.PollId)
            ?? throw new NotFoundException($"Poll with id - {pollDeleted.PollId} not found.");

        await repository.DeleteAndSaveAsync(poll);

        await userStatsRepository.DecrementCreatedPollsCountAsync(pollDeleted.UserId);

        logger.LogInformation("Poll with ID {PollId} deleted", pollDeleted.PollId);
    }
}
