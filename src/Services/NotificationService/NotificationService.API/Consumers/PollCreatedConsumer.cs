using Common.Contracts.Poll;
using Common.Repositories;
using MassTransit;
using NotificationService.API.Models;
using NotificationService.API.Repositories;

namespace NotificationService.API.Consumers;

public class PollCreatedConsumer(ILogger<PollCreatedConsumer> logger,
    IRepository<PollWithVotes, Guid> pollRepository,
    IUserStatsRepository userStatsRepository) : IConsumer<PollCreated>
{
    public async Task Consume(ConsumeContext<PollCreated> context)
    {
        var pollCreated = context.Message;

        var pollOptionsToCreate = pollCreated.OptionIds.Select(id => new PollOptionWithVotes()
        {
            OptionId = id
        }).ToList();

        await pollRepository.CreateAndSaveAsync(new PollWithVotes
        {
            Id = pollCreated.PollId,
            Options = pollOptionsToCreate
        });

        await userStatsRepository.IncrementCreatedPollsCountAsync(pollCreated.UserId);

        logger.LogInformation("Poll with ID {PollId} created", pollCreated.PollId);
    }
}
