using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMSSqlDb<TContext>(this IServiceCollection services,
        string? connectionString)
            where TContext : DbContext
    {
        if (connectionString is null)
            throw new InvalidConfigurationException("Invalid connection string.");

        services.AddDbContext<TContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }
}
