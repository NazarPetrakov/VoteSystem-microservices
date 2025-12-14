using Common.Contracts.Vote;
using MassTransit;
using NotificationService.API.Repositories;

namespace NotificationService.API.Consumers;

public class VoteDeletedConsumer(ILogger<VoteDeletedConsumer> logger, IVoteCounterRepository voteCounterRepository) : IConsumer<VoteDeleted>
{
    public async Task Consume(ConsumeContext<VoteDeleted> context)
    {
        var voteDeleted = context.Message;

        await voteCounterRepository.DecrementVoteCountAsync(voteDeleted.PollId, voteDeleted.PollOptionId);

        logger.LogInformation($"{typeof(VoteDeletedConsumer)}: vote id - {voteDeleted.VoteId}");
    }
}
