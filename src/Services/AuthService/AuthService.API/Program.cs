using AuthService.API.Data;
using AuthService.API.Extensions;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services
    .AddIdentityDb(configuration)
    .AddCommonOptions(configuration)
    .AddAppServices()
    .AddAppAuthentication(configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var provider = scope.ServiceProvider;
    try
    {
        var seeder = provider.GetRequiredService<Seeder>();
        var context = provider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();

        await seeder.SeedRolesAsync();
        await seeder.SeedUsersAsync();

    }
    catch (Exception ex)
    {
        var logger = provider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during migration");
        throw;
    }
}

app.Run();
