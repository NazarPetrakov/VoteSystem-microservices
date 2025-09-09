using VoteService.API.Contracts.Dtos;
using VoteService.API.Models;

namespace VoteService.Tests.Factories;

public static class VoteDataFactory
{
    public static List<Vote> CreateVotes(int count = 2)
    {
        return Enumerable.Range(1, count).Select(i => new Vote
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = i,
            PollId = Guid.NewGuid(),
            PollOptionId = Guid.NewGuid(),

        }).ToList();
    }
    public static List<Vote> CreateVotes(Guid pollId, Guid pollOptionId, int count = 3)
    {
        return Enumerable.Range(1, count).Select(i => new Vote
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = i,
            PollId = pollId,
            PollOptionId = pollOptionId,
        }).ToList();
    }
    public static Vote CreateVote()
    {
        return new Vote
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = 1,
            PollId = Guid.NewGuid(),
            PollOptionId = Guid.NewGuid(),
        };
    }
    public static Vote CreateVote(Guid pollId, Guid pollOptionId)
    {
        return new Vote
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = 1,
            PollId = pollId,
            PollOptionId = pollOptionId,
        };
    }
    public static Guid GuidForNumber(int number)
    {
        var bytes = new byte[16];
        BitConverter.GetBytes(number).CopyTo(bytes, 0);
        return new Guid(bytes);
    }
    public static CreateVoteRequest GetCreateVoteRequest(Guid pollId, Guid pollOptionId) =>
        new(1, pollId, pollOptionId);
    // public static CreatePollRequest GetCreatePollRequest()
    // {
    //     var pollOptions = new List<CreatePollOptionFromPollDto>
    //     {
    //         new("option 1"),
    //         new("option 2")
    //     };

    //     return new CreatePollRequest("create poll", "create poll description",
    //         1, false, pollOptions);
    // }
    // public static UpdatePollRequest GetUpdatePollRequest()
    // {
    //     return new UpdatePollRequest(Guid.NewGuid(), "update poll", "update poll description",
    //         1, false);
    // }
    // public static UpdatePollOptionRequest GetUpdatePollOptionRequest()
    // {
    //     return new UpdatePollOptionRequest(Guid.NewGuid(), "update poll option", 2,
    //         Guid.NewGuid());
    // }
    public static PollOptionCache CreatePollOption(Guid pollId = default)
    {
        return new PollOptionCache
        {
            Id = Guid.NewGuid(),
            PollId = pollId,
        };
    }
    public static List<PollOptionCache> CreatePollOptions(Guid pollId, int count = 3)
    {
        return Enumerable.Range(1, count).Select(i => new PollOptionCache
        {
            Id = Guid.NewGuid(),
            PollId = pollId,
        }).ToList();
    }
    public static PollCache CreatePoll(bool isClosed = false, int optionsCount = 2)
    {
        var pollId = Guid.NewGuid();

        return new PollCache
        {
            Id = pollId,
            IsClosed = isClosed,
            PollOptions = Enumerable.Range(1, optionsCount).Select(i => new PollOptionCache
            {
                Id = Guid.NewGuid(),
                PollId = pollId
            }).ToList()
        };
    }
    // public static List<Poll> CreatePolls(int count = 3)
    // {
    //     var polls = Enumerable.Range(1, count - 1).Select(i => new Poll
    //     {
    //         Id = Guid.NewGuid(),
    //         CreatedAt = DateTime.UtcNow,
    //         Title = $"poll {i}",
    //         Description = $"description {i}",
    //         CreatedByUserId = i,
    //         IsClosed = false,
    //     }).ToList();
    //     polls.Add(new Poll
    //     {
    //         Id = Guid.NewGuid(),
    //         CreatedAt = DateTime.UtcNow,
    //         Title = $"poll {count}",
    //         Description = $"description {count}",
    //         CreatedByUserId = count,
    //         IsClosed = true,
    //     });
    //     return polls;
    // }
    // public static List<Poll> CreateOpenPolls(int count = 2) => Enumerable.Range(1, count).Select(i => new Poll
    // {
    //     Id = Guid.NewGuid(),
    //     CreatedAt = DateTime.UtcNow,
    //     Title = $"poll {i}",
    //     Description = $"description {i}",
    //     CreatedByUserId = i,
    //     IsClosed = false,
    // }).ToList();
}
