using Microsoft.AspNetCore.SignalR;
using NotificationService.API.DTOs;
using NotificationService.API.Interfaces;

namespace NotificationService.API.Hubs;

public class VoteHub(ILogger<VoteHub> logger) : Hub<IVoteClient>
{
    public override Task OnConnectedAsync()
    {
        logger.LogInformation("A client connected to VoteHub.");
        return base.OnConnectedAsync();
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is null)
        {
            logger.LogError(exception, "A client disconnected from ChatHub with exception");
        }
        else
        {
            logger.LogInformation("A client disconnected from ChatHub");
        }
        return base.OnDisconnectedAsync(exception);
    }
    public async Task SendVotes(string groupName, VoteCountNotification notification)
    {
        var connectionId = Context.ConnectionId;

        await Clients.Group(groupName).ReceiveVotes(notification);

        logger.LogInformation($"A client:{connectionId} received the voteCountNotification withing the group {groupName}.");
    }
    public async Task JoinToGroup(string groupName)
    {
        var connectionId = Context.ConnectionId;

        await Groups.AddToGroupAsync(connectionId, groupName);

        logger.LogInformation($"A client:{connectionId} joined the group {groupName}.");
    }
    public async Task LeaveGroup(string groupName)
    {
        var connectionId = Context.ConnectionId;

        await Groups.RemoveFromGroupAsync(connectionId, groupName);

        logger.LogInformation($"A client:{connectionId} left the group {groupName}.");
    }
}
