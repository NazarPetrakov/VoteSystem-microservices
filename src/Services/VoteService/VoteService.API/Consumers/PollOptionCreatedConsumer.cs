using Common.Contracts.PollOption;
using Common.Repositories;
using MassTransit;
using VoteService.API.Exceptions;
using VoteService.API.Models;

namespace VoteService.API.Consumers;

public class PollOptionCreatedConsumer(ILogger<PollOptionCreatedConsumer> logger,
    IRepository<PollOptionCache, Guid> pollOptionRepository,
    IRepository<PollCache, Guid> pollRepository) : IConsumer<PollOptionCreated>
{
    public async Task Consume(ConsumeContext<PollOptionCreated> context)
    {
        var pollOptionCreated = context.Message;

        var pollCache = await pollRepository.GetAsync(pollOptionCreated.PollId)
            ?? throw new NotFoundException($"Poll with id - {pollOptionCreated.PollId} not found.");

        await pollOptionRepository.CreateAndSaveAsync(new PollOptionCache
        {
            Id = pollOptionCreated.PollOptionId,
            PollId = pollCache.Id
        });

        logger.LogInformation("Poll option with ID {PollOptionId} created for poll with ID {PollId}", 
            pollOptionCreated.PollOptionId, pollOptionCreated.PollId);
    }
}
