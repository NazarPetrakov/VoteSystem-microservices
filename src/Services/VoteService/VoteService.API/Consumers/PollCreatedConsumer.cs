using Common.Contracts.Poll;
using Common.Repositories;
using MassTransit;
using VoteService.API.Models;

namespace VoteService.API.Consumers;

public class PollCreatedConsumer(ILogger<PollCreatedConsumer> logger,
    IRepository<PollCache, Guid> pollRepository,
    IRepository<PollOptionCache, Guid> pollOptionRepository) : IConsumer<PollCreated>
{
    public async Task Consume(ConsumeContext<PollCreated> context)
    {
        var pollCreated = context.Message;

        var createdPoll = await pollRepository.CreateAndSaveAsync(new PollCache
        {
            Id = pollCreated.PollId,
            IsClosed = pollCreated.IsClosed
        });

        if (createdPoll.Id != pollCreated.PollId)
        {
            logger.LogWarning("Poll with id - {PollId} was not created", pollCreated.PollId);
        }

        foreach (var optionId in pollCreated.OptionIds)
        {
            await pollOptionRepository.CreateAndSaveAsync(new PollOptionCache()
            {
                Id = optionId,
                PollId = createdPoll.Id
            });
        }

        logger.LogInformation($"{typeof(PollCreatedConsumer)}: poll id - {pollCreated.PollId}");
    }
}
