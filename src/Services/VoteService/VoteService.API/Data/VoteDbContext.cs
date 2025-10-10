using Microsoft.EntityFrameworkCore;
using VoteService.API.Models;

namespace VoteService.API.Data;

public class VoteDbContext(DbContextOptions<VoteDbContext> options) : DbContext(options)
{
    public DbSet<Vote> Votes { get; set; }
    public DbSet<PollCache> PollsCache { get; set; }
    public DbSet<PollOptionCache> PollOptionsCache { get; set; }
    public DbSet<UserCache> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vote>()
            .HasOne(v => v.Poll)
            .WithMany(p => p.Votes)
            .HasForeignKey(v => v.PollId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Vote>()
            .HasOne(v => v.PollOption)
            .WithMany(po => po.Votes)
            .HasForeignKey(v => v.PollOptionId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PollOptionCache>()
            .HasOne(po => po.Poll)
            .WithMany(p => p.PollOptions)
            .HasForeignKey(po => po.PollId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
