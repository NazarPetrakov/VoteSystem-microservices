using Microsoft.EntityFrameworkCore;
using PollService.API.Models;

namespace PollService.API.Data;

public class PollDbContext(DbContextOptions<PollDbContext> options)
    : DbContext(options)
{
    public DbSet<Poll> Polls { get; set; }
    public DbSet<PollOption> PollOptions { get; set; }
    public DbSet<UserCache> Users { get; set; }
}
