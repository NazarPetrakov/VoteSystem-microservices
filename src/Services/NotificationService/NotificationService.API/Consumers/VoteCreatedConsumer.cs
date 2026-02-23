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

public class VoteCreatedConsumer(ILogger<VoteCreatedConsumer> logger,
    IHubContext<VoteHub, IVoteClient> hubContext,
    IVoteCounterRepository voteCounterRepository,
    IUserStatsRepository userStatsRepository) : IConsumer<VoteCreated>
{
    public async Task Consume(ConsumeContext<VoteCreated> context)
    {
        var voteCreated = context.Message;

        var updatedPollWithVotes = await voteCounterRepository
            .IncrementVoteCountAsync(voteCreated.PollId, voteCreated.PollOptionId)
                ?? throw new NotFoundException($"Poll with id:{voteCreated.PollId} not found.");

        await SendVoteCountNotification(updatedPollWithVotes, voteCreated.PollOptionId);

        await userStatsRepository.IncrementVotedPollsCountAsync(voteCreated.UserId);

        logger.LogInformation("Vote with ID {VoteId} created by user {UserId}", voteCreated.VoteId, voteCreated.UserId);
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
