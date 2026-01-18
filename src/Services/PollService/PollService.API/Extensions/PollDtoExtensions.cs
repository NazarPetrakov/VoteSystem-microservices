using PollService.API.Helpers;
using PollService.API.Models;

namespace PollService.API.Extensions;

public static class PollDtoExtensions
{
    public static PollResponse ToDto(this Poll poll)
    {
        return new PollResponse(poll.Id,
            poll.Title,
            poll.Topic,
            poll.UserId,
            poll.User?.UserName ?? "",
            poll.IsClosed,
            poll.Duration,
            poll.CreatedAt,
            poll.UpdatedAt,
            poll.EndTime,
            poll.IsExpired,
            poll.IsActive,
            poll.PollOptions.Select(po => po.ToDto()).ToList());
    }
    public static Poll ToEntity(this CreatePollRequest pollCreate, int userId)
    {
        var timeCount = pollCreate.TimeCount;
        TimeSpan timeSpan = pollCreate.TimeUnit switch
        {
            "minute" => TimeSpan.FromMinutes(timeCount),
            "hour" => TimeSpan.FromHours(timeCount),
            "day" => TimeSpan.FromDays(timeCount),
            "week" => TimeSpan.FromDays(timeCount * 7),
            _ => throw new ArgumentException("Invalid TimeUnit")
        };
        return new Poll()
        {
            Title = pollCreate.Title,
            Topic = pollCreate.Topic,
            UserId = userId,
            Duration = timeSpan,
            IsClosed = pollCreate.IsClosed,
        };
    }
    public static Poll ToEntity(this UpdatePollRequest pollUpdate, Poll existedPoll)
    {
        return new Poll()
        {
            Id = pollUpdate.Id,
            Title = pollUpdate.Title ?? existedPoll.Title,
            Topic = pollUpdate.Topic ?? existedPoll.Topic,
            UserId = existedPoll.UserId,
            IsClosed = pollUpdate.IsClosed ?? existedPoll.IsClosed,
            CreatedAt = existedPoll.CreatedAt,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }
}
