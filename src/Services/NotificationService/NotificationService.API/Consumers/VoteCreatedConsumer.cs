using Common.Contracts.Vote;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.API.DTOs;
using NotificationService.API.Exceptions;
using NotificationService.API.Hubs;
using NotificationService.API.Interfaces;
using NotificationService.API.Repositories;

namespace NotificationService.API.Consumers;

public class VoteCreatedConsumer(ILogger<VoteCreatedConsumer> logger,
    IVoteCounterRepository voteCounterRepository, IHubContext<VoteHub, IVoteClient> hubContext) : IConsumer<VoteCreated>
{
    public async Task Consume(ConsumeContext<VoteCreated> context)
    {
        var voteCreated = context.Message;

        var updatedVoteCount = await voteCounterRepository.IncrementVoteCountAsync(voteCreated.PollId, voteCreated.PollOptionId);

        var votedOption = updatedVoteCount.Options
            .FirstOrDefault(o => o.OptionId == voteCreated.PollOptionId)
                ?? throw new NotFoundException($"Poll option with id:{voteCreated.PollOptionId} not found.");

        var notification = new VoteCountNotification
        {
            PollId = updatedVoteCount.Id,
            PollOptionId = votedOption.OptionId,
            PollVotesCount = updatedVoteCount.TotalVotes,
            PollOptionVotesCount = votedOption.VoteCount
        };

        await hubContext.Clients.Group(updatedVoteCount.Id.ToString()).ReceiveVotes(notification);

        logger.LogInformation("Client withing the group: {GroupName} received notification", updatedVoteCount.Id.ToString());

        logger.LogInformation("Vote with ID {VoteId} created", voteCreated.VoteId);
    }
}
