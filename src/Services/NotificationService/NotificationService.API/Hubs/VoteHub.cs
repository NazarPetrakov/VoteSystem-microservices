using Microsoft.AspNetCore.SignalR;
using NotificationService.API.DTOs;
using NotificationService.API.Interfaces;

namespace NotificationService.API.Hubs;

public class VoteHub(ILogger<VoteHub> logger) : Hub<IVoteClient>
{
    public override Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;

        logger.LogInformation("A client:{ConnectionId} connected to VoteHub.", connectionId);
        return base.OnConnectedAsync();
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;

        if (exception is null)
        {
            logger.LogInformation("A client:{ConnectionId} disconnected from ChatHub", connectionId);
        }
        else
        {
            logger.LogError(exception, "A client:{ConnectionId} disconnected from ChatHub with exception", connectionId);
        }
        return base.OnDisconnectedAsync(exception);
    }
    public async Task SendVotes(string groupName, VoteCountNotification notification)
    {
        var connectionId = Context.ConnectionId;

        await Clients.Group(groupName).ReceiveVotes(notification);

        logger.LogInformation("A client:{ConnectionId} received the voteCountNotification withing the group {GroupName}.", connectionId, groupName);
    }
    public async Task JoinToGroup(string groupName)
    {
        var connectionId = Context.ConnectionId;

        await Groups.AddToGroupAsync(connectionId, groupName);

        logger.LogInformation("A client:{ConnectionId} joined the group {GroupName}.", connectionId, groupName);
    }
    public async Task LeaveGroup(string groupName)
    {
        var connectionId = Context.ConnectionId;

        await Groups.RemoveFromGroupAsync(connectionId, groupName);

        logger.LogInformation("A client:{ConnectionId} left the group {GroupName}.", connectionId, groupName);
    }
}
