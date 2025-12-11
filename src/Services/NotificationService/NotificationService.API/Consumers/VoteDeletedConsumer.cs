using Common.Contracts.Vote;
using Common.Repositories;
using MassTransit;
using NotificationService.API.Exceptions;
using NotificationService.API.Models;

namespace NotificationService.API.Consumers;

public class VoteDeletedConsumer(ILogger<VoteDeletedConsumer> logger, IRepository<NotificationPoll, Guid> repository) : IConsumer<VoteDeleted>
{
    public async Task Consume(ConsumeContext<VoteDeleted> context)
    {
        var voteDeleted = context.Message;

        var poll = await repository.GetAsync(voteDeleted.PollId)
            ?? throw new NotFoundException($"Poll with id - {voteDeleted.PollId} not found.");

        var pollOption = poll.Options.FirstOrDefault(o => o.OptionId == voteDeleted.PollOptionId)
            ?? throw new NotFoundException($"Poll option with id - {voteDeleted.PollOptionId} not found in poll with id - {voteDeleted.PollId}.");

        if (poll.TotalVotes <= 0 || pollOption.VoteCount <= 0)
        {
            logger.LogWarning($"{typeof(VoteDeletedConsumer)}: vote id - {voteDeleted.VoteId} has inconsistent vote counts. TotalVotes: {poll.TotalVotes}, OptionVoteCount: {pollOption.VoteCount}");
            return;
        }

        poll.TotalVotes--;
        pollOption.VoteCount--;

        await repository.UpdateAndSaveAsync(poll);

        logger.LogInformation($"{typeof(VoteDeletedConsumer)}: vote id - {voteDeleted.VoteId}");
    }
}
