using Microsoft.Extensions.Hosting;
using Serilog;

namespace Common.Extensions;

public static class LoggingExtensions
{
    public static IHostBuilder UseCommonSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, services, loggerConfig) =>
        {
            loggerConfig
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services);
        });
    }
}
