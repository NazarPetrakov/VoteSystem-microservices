using Common.Contracts.Poll;
using Common.Repositories;
using MassTransit;
using VoteService.API.Models;

namespace VoteService.API.Consumers;

public class PollCreatedConsumer(ILogger<PollCreatedConsumer> logger,
    IRepository<PollCache, Guid> pollRepository) : IConsumer<PollCreated>
{
    public async Task Consume(ConsumeContext<PollCreated> context)
    {
        var pollCreated = context.Message;

        await pollRepository.CreateAndSaveAsync(new PollCache
        {
            Id = pollCreated.PollId,
            IsClosed = pollCreated.IsClosed
        });

        logger.LogInformation($"{typeof(PollCreatedConsumer)}: poll id - {pollCreated.PollId}");
    }
}
