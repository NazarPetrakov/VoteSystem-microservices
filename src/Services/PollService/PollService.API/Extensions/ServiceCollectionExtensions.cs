using Common.Options;
using Common.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PollService.API.Consumers;
using PollService.API.Data;
using PollService.API.Interfaces;
using PollService.API.Orchestrators;
using PollService.API.Publishers;
using PollService.API.Repositories;

namespace PollService.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
    {
        services.AddMassTransit(configure =>
        {
            configure.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("pollservice", false));

            configure.AddConsumer<UserCreatedConsumer>();
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
    public static IServiceCollection AddPollDbContext(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<PollDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        });

        return services;
    }
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<,>), typeof(MSSqlRepository<,>));
        services.AddScoped<IPollOrchestrator, PollOrchestrator>();
        services.AddScoped<IPollOptionOrchestrator, PollOptionOrchestrator>();

        services.AddScoped<IPollPublisher, PollPublisher>();

        return services;
    }
}
