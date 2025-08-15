using Microsoft.EntityFrameworkCore;
using PollService.API.Data;
using PollService.API.Interfaces;
using PollService.API.Orchestrators;
using PollService.API.Repositories;

namespace PollService.API.Extensions;

public static class ServiceCollectionExtensions
{
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
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IPollOrchestrator, PollOrchestrator>();
        services.AddScoped<IPollOptionOrchestrator, PollOptionOrchestrator>();

        return services;
    }
}
