using Common.Contracts.PollOption;
using Common.Repositories;
using MassTransit;
using NotificationService.API.Exceptions;
using NotificationService.API.Models;

namespace NotificationService.API.Consumers;

public class PollOptionCreatedConsumer(ILogger<PollOptionCreatedConsumer> logger,
    IRepository<NotificationPoll, Guid> repository) : IConsumer<PollOptionCreated>
{
    public async Task Consume(ConsumeContext<PollOptionCreated> context)
    {
        var pollOptionCreated = context.Message;

        var poll = await repository.GetAsync(pollOptionCreated.PollId)
            ?? throw new NotFoundException($"Poll with id - {pollOptionCreated.PollId} not found.");

        poll.Options.Add(new NotificationPollOption
        {
            OptionId = pollOptionCreated.PollOptionId
        });

        await repository.UpdateAndSaveAsync(poll);

        logger.LogInformation($"{typeof(PollOptionCreatedConsumer)}: poll id - {pollOptionCreated.PollId}");
    }
}
