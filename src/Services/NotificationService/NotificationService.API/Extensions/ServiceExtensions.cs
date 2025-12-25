using Common.Options;
using Common.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.Configuration;
using MongoDB.Driver;
using NotificationService.API.Consumers;
using NotificationService.API.Data;
using NotificationService.API.Repositories;
using NotificationService.API.Settings;

namespace NotificationService.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddMassTransit(configure =>
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

        services.AddSignalR();

        services.AddScoped(typeof(IRepository<,>), typeof(MongoRepository<,>));
        services.AddScoped<IVoteCounterRepository, VoteCounterRepository>();


        return services;
    }
    public static IServiceCollection AddDbServices(this IServiceCollection services, MongoDbSettings? mongoSettings)
    {
        if (mongoSettings is null)
        {
            throw new InvalidConfigurationException("Invalid MongoDbSettings configuration");
        }

        services.AddDbContext<AppDbContext>(cfg =>
        {
            cfg.UseMongoDB(mongoSettings?.ConnectionString ?? "", mongoSettings?.DatabaseName ?? "");
        });

        services.AddSingleton<IMongoClient>(sp =>
        {
            return new MongoClient(mongoSettings?.ConnectionString ?? "");
        });

        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoSettings?.DatabaseName ?? "");
        });

        return services;
    }
}
