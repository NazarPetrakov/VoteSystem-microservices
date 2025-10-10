using AuthService.API.Data;
using AuthService.API.Interfaces;
using AuthService.API.Models;
using AuthService.API.Options;
using AuthService.API.Orchestrators;
using AuthService.API.Publishers;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthService.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration
            .GetSection(JwtOptions.SectionName));
        services.Configure<RabbitMqOptions>(configuration
            .GetSection(RabbitMqOptions.SectionName));

        return services;
    }
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthOrchestrator, AuthOrchestrator>();
        services.AddScoped<Seeder>();
        services.AddScoped<IUserPublisher, UserPublisher>();

        services.AddMassTransit(c =>
        {
            c.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("authservice", false));

            c.UsingRabbitMq((context, configuration) =>
            {
                var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                configuration.Host(new Uri(rabbitMqOptions.Host), h =>
                {
                    h.Username(rabbitMqOptions.Username);
                    h.Password(rabbitMqOptions.Password);
                });

                configuration.ConfigureEndpoints(context);
            });
        });

        return services;
    }
    public static IServiceCollection AddIdentityDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(c =>
            c.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<AppUser, AppRole>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
        })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

}
