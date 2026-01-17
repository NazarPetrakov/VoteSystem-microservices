using Common.Contracts.Poll;
using Common.Repositories;
using MassTransit;
using NotificationService.API.Models;

namespace NotificationService.API.Consumers;

public class PollCreatedConsumer(ILogger<PollCreatedConsumer> logger,
    IRepository<NotificationPoll, Guid> pollRepository) : IConsumer<PollCreated>
{
    public async Task Consume(ConsumeContext<PollCreated> context)
    {
        var pollCreated = context.Message;

        var pollOptionsToCreate = pollCreated.OptionIds.Select(id => new NotificationPollOption()
        {
            OptionId = id
        }).ToList();

        await pollRepository.CreateAndSaveAsync(new NotificationPoll
        {
            Id = pollCreated.PollId,
            IsClosed = pollCreated.IsClosed,
            Options = pollOptionsToCreate
        });

        logger.LogInformation("Poll with ID {PollId} created", pollCreated.PollId);
    }
}
