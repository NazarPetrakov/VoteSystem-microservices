using PollService.API.Helpers;
using PollService.API.Models;

namespace PollService.Tests.Factories;

public static class PollDataFactory
{
    public static CreatePollOptionRequest GetCreatePollOptionRequest(Guid pollId) =>
        new("poll option", pollId);
    public static CreatePollRequest GetCreatePollRequest()
    {
        var pollOptions = new List<CreatePollOptionFromPollDto>
        {
            new("option 1"),
            new("option 2")
        };

        return new CreatePollRequest("create poll", "create poll description",
            1, false, pollOptions);
    }
    public static UpdatePollRequest GetUpdatePollRequest()
    {
        return new UpdatePollRequest(Guid.NewGuid(), "update poll", "update poll description",
            1, false);
    }
    public static UpdatePollOptionRequest GetUpdatePollOptionRequest()
    {
        return new UpdatePollOptionRequest(Guid.NewGuid(), "update poll option", 2,
            Guid.NewGuid());
    }
    public static PollOption CreatePollOption(Guid pollId = default)
    {
        return new PollOption
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            Text = "pollOption",
            Order = 1,
            PollId = pollId,
        };
    }
    public static List<PollOption> CreatePollOptions(int count = 3) => Enumerable.Range(1, count).Select(i => new PollOption
    {
        Id = Guid.NewGuid(),
        CreatedAt = DateTimeOffset.UtcNow,
        Text = $"option {i}",
        Order = i,
        PollId = Guid.NewGuid(),
    }).ToList();
    public static Poll CreatePoll(bool isClosed = false, int optionsCount = 2)
    {
        var pollId = Guid.NewGuid();

        return new Poll
        {
            Id = pollId,
            CreatedAt = DateTime.UtcNow,
            Title = "poll",
            Description = "description",
            CreatedByUserId = 1,
            IsClosed = isClosed,
            PollOptions = Enumerable.Range(1, optionsCount).Select(i => new PollOption
            {
                Text = $"poll option {i}",
                Order = i,
                PollId = pollId
            }).ToList()
        };
    }
    public static List<Poll> CreatePolls(int count = 3)
    {
        var polls = Enumerable.Range(1, count - 1).Select(i => new Poll
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Title = $"poll {i}",
            Description = $"description {i}",
            CreatedByUserId = i,
            IsClosed = false,
        }).ToList();
        polls.Add(new Poll
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Title = $"poll {count}",
            Description = $"description {count}",
            CreatedByUserId = count,
            IsClosed = true,
        });
        return polls;
    }
    public static List<Poll> CreateOpenPolls(int count = 2) => Enumerable.Range(1, count).Select(i => new Poll
    {
        Id = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        Title = $"poll {i}",
        Description = $"description {i}",
        CreatedByUserId = i,
        IsClosed = false,
    }).ToList();
}
