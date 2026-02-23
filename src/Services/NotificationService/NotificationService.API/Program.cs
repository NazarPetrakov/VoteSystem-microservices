using Common.Extensions;
using Common.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using NotificationService.API.Extensions;
using NotificationService.API.Hubs;
using NotificationService.API.Models;
using NotificationService.API.Repositories;
using NotificationService.API.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

BsonSerializer.RegisterSerializer(
    new GuidSerializer(GuidRepresentation.Standard)
);

var mongoSettings = builder.Configuration.GetSection(MongoDbSettings.SectionName)
    .Get<MongoDbSettings>();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection(MongoDbSettings.SectionName));

builder.Services.AddCommonOptions(builder.Configuration).AddAppServices().AddDbServices(mongoSettings);

builder.Host.UseCommonSerilog();

var app = builder.Build();

app.UseCors(c =>
{
    c.AllowAnyHeader().AllowCredentials().AllowAnyMethod().WithOrigins("http://localhost:4200");
});

app.MapHub<VoteHub>("/voteHub");

app.MapGet("api/notifications/poll/{pollId}/votes-count", async (Guid pollId, IRepository<PollWithVotes, Guid> repository) =>
{
    var result = await repository.GetAsync(pollId);

    if (result is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(result);
});

app.MapGet("api/notification/users/{userId}/stats", async (int userId, IRepository<UserStats, int> repository) =>
{
    var result = await repository.GetAsync(userId);

    if (result is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(result);
});

app.UseHttpsRedirection();

//Warmup MongoDb and EFCore connection
using (var scope = app.Services.CreateScope())
{
    var pollsRepository = scope.ServiceProvider
        .GetRequiredService<IRepository<PollWithVotes, Guid>>();

    var userStatsRepository = scope.ServiceProvider
        .GetRequiredService<IRepository<UserStats, int>>();

    await pollsRepository.GetAllQuery().AnyAsync();
    await userStatsRepository.GetAllQuery().AnyAsync();
}

app.Run();