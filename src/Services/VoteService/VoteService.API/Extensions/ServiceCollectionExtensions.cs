using Common.Options;
using Common.Repositories;
using MassTransit;
using Microsoft.Extensions.Options;
using VoteService.API.Consumers;
using VoteService.API.Interfaces;
using VoteService.API.Orchestrators;
using VoteService.API.Repositories;

namespace VoteService.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
    {
        services.AddMassTransit(configure =>
        {
            configure.SetKebabCaseEndpointNameFormatter();

            configure.AddConsumer<PollOptionCreatedConsumer>(c =>
            {
                c.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(2)));
            });
            configure.AddConsumer<PollCreatedConsumer>();
            configure.AddConsumer<PollDeletedConsumer>();
            configure.AddConsumer<PollOptionDeletedConsumer>();

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

        return services;
    }
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<,>), typeof(MSSqlRepository<,>));
        services.AddScoped<IVoteOrchestrator, VoteOrchestrator>();

        return services;
    }
}
