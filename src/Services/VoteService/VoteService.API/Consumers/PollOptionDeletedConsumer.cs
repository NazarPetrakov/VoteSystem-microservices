using Common.Contracts.PollOption;
using Common.Repositories;
using MassTransit;
using VoteService.API.Exceptions;
using VoteService.API.Models;

namespace VoteService.API.Consumers;

public class PollOptionDeletedConsumer(ILogger<PollOptionDeletedConsumer> logger,
    IRepository<PollOptionCache, Guid> pollOptionRepository) : IConsumer<PollOptionDeleted>
{
    public async Task Consume(ConsumeContext<PollOptionDeleted> context)
    {
        var pollOptionDeleted = context.Message;

        var pollOptionCache = await pollOptionRepository.GetAsync(pollOptionDeleted.PollOptionId)
            ?? throw new NotFoundException($"Poll option with id - {pollOptionDeleted.PollOptionId} not found.");

        await pollOptionRepository.DeleteAndSaveAsync(pollOptionCache);

        logger.LogInformation($"{typeof(PollOptionDeletedConsumer)}: poll option id - {pollOptionCache.Id}");
    }
}
