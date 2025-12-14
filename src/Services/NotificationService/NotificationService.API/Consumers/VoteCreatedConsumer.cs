using Common.Contracts.Vote;
using MassTransit;
using NotificationService.API.Repositories;

namespace NotificationService.API.Consumers;

public class VoteCreatedConsumer(ILogger<VoteCreatedConsumer> logger, IVoteCounterRepository voteCounterRepository) : IConsumer<VoteCreated>
{
    public async Task Consume(ConsumeContext<VoteCreated> context)
    {
        var voteCreated = context.Message;

        await voteCounterRepository.IncrementVoteCountAsync(voteCreated.PollId, voteCreated.PollOptionId);

        logger.LogInformation($"{typeof(VoteCreatedConsumer)}: vote id - {voteCreated.VoteId}");
    }
}
