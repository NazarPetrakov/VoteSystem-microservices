using Microsoft.EntityFrameworkCore;
using NotificationService.API.Data;
using NotificationService.API.Models;
using NotificationService.API.Settigns;

var builder = WebApplication.CreateBuilder(args);

// TODO: DTO for poll 
// TODO: DTO for pollOption 
// TODO: Add massTransit, consumers for poll and pollOptions
// TODO: event contracts for voteCreated voteDeleted maybe


var mongoSettings = builder.Configuration.GetSection(MongoDbSettings.SectionName).Get<MongoDbSettings>();

builder.Services.AddDbContext<AppDbContext>(cfg =>
{
    cfg.UseMongoDB(mongoSettings?.ConnectionString ?? "", mongoSettings?.DatabaseName ?? "");
});

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/notifications", async (AppDbContext appDbContext) =>
{
    appDbContext.Polls.Add(new NotificationPoll
    {
        Id = new Guid("00000000-0000-0000-0000-000000000002"),
        Title = "First",
        IsClosed = false,
        TotalVotes = 0,
        Options = new List<NotificationPollOption>
        {
            new NotificationPollOption
            {
                Text = "First option",
                VoteCount = 1
            },
            new NotificationPollOption
            {
                Text = "Second option",
                VoteCount = 2
            }
        }
    });
    await appDbContext.SaveChangesAsync();

    return Results.Ok();
});
app.MapPost("/notifications/options", async (AppDbContext appDbContext) =>
{
    var poll = appDbContext.Polls.FirstOrDefault(p => p.Id == new Guid("00000000-0000-0000-0000-000000000002"));

    poll.Options.Add(new NotificationPollOption { Id = new Guid("00000000-0000-0000-0000-000000000003"), Text = "Third option", VoteCount = 0 });

    await appDbContext.SaveChangesAsync();

    return Results.Ok();
});

app.MapGet("/notifications", (AppDbContext appDbContext) =>
{
    var result = appDbContext.Polls.ToList();
    return Results.Ok(result);
});

app.Run();