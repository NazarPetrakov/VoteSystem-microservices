using Common.Extensions;
using Common.Options;
using Common.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NotificationService.API.Consumers;
using NotificationService.API.Data;
using NotificationService.API.Models;
using NotificationService.API.Repositories;
using NotificationService.API.Settings;

var builder = WebApplication.CreateBuilder(args);

var mongoSettings = builder.Configuration.GetSection(MongoDbSettings.SectionName).Get<MongoDbSettings>();

builder.Services.AddCommonOptions(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>(cfg =>
{
    cfg.UseMongoDB(mongoSettings?.ConnectionString ?? "", mongoSettings?.DatabaseName ?? "");
});

builder.Services.AddMassTransit(configure =>
        {
            configure.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("notifyservice", false));

            configure.AddConsumer<PollOptionCreatedConsumer>(c =>
            {
                c.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(5)));
            });
            configure.AddConsumer<PollCreatedConsumer>();
            configure.AddConsumer<PollDeletedConsumer>();
            configure.AddConsumer<VoteCreatedConsumer>();
            configure.AddConsumer<VoteDeletedConsumer>();
            configure.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                cfg.Host(new Uri(rabbitMqOptions.Host), h =>
                {
                    h.Username(rabbitMqOptions.Username);
                    h.Password(rabbitMqOptions.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

builder.Services.AddScoped(typeof(IRepository<,>), typeof(MongoRepository<,>));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/notifications", async (AppDbContext appDbContext) =>
{
    appDbContext.Polls.Add(new NotificationPoll
    {
        Id = new Guid("00000000-0000-0000-0000-000000000003"),
        Title = "Hello from Notification Service",
        IsClosed = false,
        TotalVotes = 110,
        Options = new List<NotificationPollOption>
        {
            new NotificationPollOption
            {
                Text = "First option",
                VoteCount = 10
            },
            new NotificationPollOption
            {
                Text = "Second option",
                VoteCount = 100
            }
        }
    });
    await appDbContext.SaveChangesAsync();

    return Results.Ok();
});


app.MapGet("/notifications", (AppDbContext appDbContext) =>
{
    var result = appDbContext.Polls.ToList();
    return Results.Ok(result);
});

app.Run();