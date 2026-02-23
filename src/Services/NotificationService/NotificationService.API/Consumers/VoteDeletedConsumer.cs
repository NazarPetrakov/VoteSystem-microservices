using Common.Contracts.Vote;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.API.DTOs;
using NotificationService.API.Exceptions;
using NotificationService.API.Hubs;
using NotificationService.API.Interfaces;
using NotificationService.API.Models;
using NotificationService.API.Repositories;

namespace NotificationService.API.Consumers;

public class VoteDeletedConsumer(ILogger<VoteDeletedConsumer> logger,
    IHubContext<VoteHub, IVoteClient> hubContext,
    IVoteCounterRepository voteCounterRepository,
    IUserStatsRepository userStatsRepository) : IConsumer<VoteDeleted>
{
    public async Task Consume(ConsumeContext<VoteDeleted> context)
    {
        var voteDeleted = context.Message;

        var updatedPollWithVotes = await voteCounterRepository
            .DecrementVoteCountAsync(voteDeleted.PollId, voteDeleted.PollOptionId)
                ?? throw new NotFoundException($"Poll with id:{voteDeleted.PollId} not found."); ;

        await SendVoteCountNotification(updatedPollWithVotes, voteDeleted.PollOptionId);

        await userStatsRepository.DecrementVotedPollsCountAsync(voteDeleted.UserId);

        logger.LogInformation("Vote with ID {VoteId} deleted by user {UserId}", voteDeleted.VoteId, voteDeleted.UserId);
    }
    private async Task SendVoteCountNotification(PollWithVotes updatedPollWithVotes, Guid votedOptionId)
    {
        var votedOption = updatedPollWithVotes.Options
            .FirstOrDefault(o => o.OptionId == votedOptionId)
                ?? throw new NotFoundException($"Poll option with id:{votedOptionId} not found.");

        var notification = new VoteCountNotification
        {
            PollId = updatedPollWithVotes.Id,
            PollOptionId = votedOption.OptionId,
            PollVotesCount = updatedPollWithVotes.TotalVotes,
            PollOptionVotesCount = votedOption.VoteCount
        };

        await hubContext.Clients.Group(updatedPollWithVotes.Id.ToString()).ReceiveVotes(notification);

        logger.LogInformation("Client withing the group: {GroupName} received notification", updatedPollWithVotes.Id.ToString());
    }
}
