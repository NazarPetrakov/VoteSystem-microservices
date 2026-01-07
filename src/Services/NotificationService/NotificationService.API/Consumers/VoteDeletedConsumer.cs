using Common.Contracts.Vote;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.API.DTOs;
using NotificationService.API.Exceptions;
using NotificationService.API.Hubs;
using NotificationService.API.Interfaces;
using NotificationService.API.Repositories;

namespace NotificationService.API.Consumers;

public class VoteDeletedConsumer(ILogger<VoteDeletedConsumer> logger,
    IVoteCounterRepository voteCounterRepository, IHubContext<VoteHub, IVoteClient> hubContext) : IConsumer<VoteDeleted>
{
    public async Task Consume(ConsumeContext<VoteDeleted> context)
    {
        var voteDeleted = context.Message;

        var updatedVoteCount = await voteCounterRepository.DecrementVoteCountAsync(voteDeleted.PollId, voteDeleted.PollOptionId);

        var votedOption = updatedVoteCount.Options
            .FirstOrDefault(o => o.OptionId == voteDeleted.PollOptionId)
                ?? throw new NotFoundException($"Poll option with id:{voteDeleted.PollOptionId} not found.");

        var notification = new VoteCountNotification
        {
            PollId = updatedVoteCount.Id,
            PollOptionId = votedOption.OptionId,
            PollVotesCount = updatedVoteCount.TotalVotes,
            PollOptionVotesCount = votedOption.VoteCount
        };

        await hubContext.Clients.Group(updatedVoteCount.Id.ToString()).ReceiveVotes(notification);

        logger.LogInformation("Client withing the group: {GroupName} received notification", updatedVoteCount.Id.ToString());

        logger.LogInformation($"{typeof(VoteDeletedConsumer)}: vote id - {voteDeleted.VoteId}");
    }
}
