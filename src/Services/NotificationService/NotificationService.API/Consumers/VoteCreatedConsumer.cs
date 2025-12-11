using Common.Contracts.Vote;
using Common.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using NotificationService.API.Exceptions;
using NotificationService.API.Models;

namespace NotificationService.API.Consumers;

public class VoteCreatedConsumer(ILogger<VoteCreatedConsumer> logger, IRepository<NotificationPoll, Guid> repository) : IConsumer<VoteCreated>
{
    public async Task Consume(ConsumeContext<VoteCreated> context)
    {
        var voteCreated = context.Message;

        var poll = await repository.GetAsync(voteCreated.PollId)
            ?? throw new NotFoundException($"Poll with id - {voteCreated.PollId} not found.");

        var pollOption = poll.Options.FirstOrDefault(o => o.OptionId == voteCreated.PollOptionId)
            ?? throw new NotFoundException($"Poll option with id - {voteCreated.PollOptionId} not found in poll with id - {voteCreated.PollId}.");

        poll.TotalVotes++;
        pollOption.VoteCount++;

        await repository.UpdateAndSaveAsync(poll);

        logger.LogInformation($"{typeof(VoteCreatedConsumer)}: vote id - {voteCreated.VoteId}");
    }
}
