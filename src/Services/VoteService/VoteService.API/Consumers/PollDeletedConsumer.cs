using Common.Contracts.Poll;
using Common.Repositories;
using MassTransit;
using VoteService.API.Exceptions;
using VoteService.API.Models;

namespace VoteService.API.Consumers;

public class PollDeletedConsumer(ILogger<PollDeletedConsumer> logger,
    IRepository<PollCache, Guid> pollRepository) : IConsumer<PollDeleted>
{
    public async Task Consume(ConsumeContext<PollDeleted> context)
    {
        var pollDeleted = context.Message;

        var pollCache = await pollRepository.GetAsync(pollDeleted.PollId)
            ?? throw new NotFoundException($"Poll with id - {pollDeleted.PollId} not found.");

        await pollRepository.DeleteAndSaveAsync(pollCache);

        logger.LogInformation($"{typeof(PollDeletedConsumer)}: poll id - {pollDeleted.PollId}");
    }
}
