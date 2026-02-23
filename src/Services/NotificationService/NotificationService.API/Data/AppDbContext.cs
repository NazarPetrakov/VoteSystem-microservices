using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.EntityFrameworkCore.Extensions;
using NotificationService.API.Models;
using NotificationService.API.Settings;

namespace NotificationService.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, IOptions<MongoDbSettings> settings) : DbContext(options)
{
    public DbSet<PollWithVotes> Polls { get; init; }
    public DbSet<UserStats> UserStats { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PollWithVotes>()
            .ToCollection(settings.Value.PollWithTotalVotesCollectionName);
        modelBuilder.Entity<UserStats>()
            .ToCollection(settings.Value.UserStatsCollectionName);
    }
}
