using Common.Extensions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using NotificationService.API.Extensions;
using NotificationService.API.Hubs;
using NotificationService.API.Settings;

var builder = WebApplication.CreateBuilder(args);

BsonSerializer.RegisterSerializer(
    new GuidSerializer(GuidRepresentation.Standard)
);

var mongoSettings = builder.Configuration.GetSection(MongoDbSettings.SectionName)
    .Get<MongoDbSettings>();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection(MongoDbSettings.SectionName));

builder.Services.AddCommonOptions(builder.Configuration).AddAppServices().AddDbServices(mongoSettings);

var app = builder.Build();

app.MapHub<VoteHub>("/voteHub");

app.UseHttpsRedirection();

app.Run();