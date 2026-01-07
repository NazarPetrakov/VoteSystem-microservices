using Common.Extensions;
using Common.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using NotificationService.API.Extensions;
using NotificationService.API.Hubs;
using NotificationService.API.Models;
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

var app = builder.Build();

app.UseCors(c =>
{
    c.AllowAnyHeader().AllowCredentials().AllowAnyMethod().WithOrigins("http://localhost:4200");
});

app.MapHub<VoteHub>("/voteHub");

app.MapGet("api/notifications/poll/{pollId}/votes", async (Guid pollId, IRepository<NotificationPoll, Guid> repository) =>
{
    var result = await repository.GetAsync(pollId);
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
    var repository = scope.ServiceProvider
        .GetRequiredService<IRepository<NotificationPoll, Guid>>();

    await repository.GetAllQuery().AnyAsync();
}

app.Run();