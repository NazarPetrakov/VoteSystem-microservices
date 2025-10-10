using PollService.API.Helpers;
using PollService.API.Models;

namespace PollService.API.Extensions;

public static class PollDtoExtensions
{
    public static PollResponse ToDto(this Poll poll)
    {
        return new PollResponse(poll.Id,
            poll.Title,
            poll.Description,
            poll.UserId,
            poll.IsClosed,
            poll.CreatedAt,
            poll.UpdatedAt,
            poll.PollOptions.Select(po => po.ToDto()).ToList());
    }
    public static Poll ToEntity(this CreatePollRequest pollCreate)
    {
        return new Poll()
        {
            Title = pollCreate.Title,
            Description = pollCreate.Description,
            UserId = pollCreate.UserId,
            IsClosed = pollCreate.IsClosed,
        };
    }
    public static Poll ToEntity(this UpdatePollRequest pollUpdate, Poll existedPoll)
    {
        return new Poll()
        {
            Id = pollUpdate.Id,
            Title = pollUpdate.Title ?? existedPoll.Title,
            Description = pollUpdate.Description ?? existedPoll.Description,
            UserId = pollUpdate.UserId ?? existedPoll.UserId,
            IsClosed = pollUpdate.IsClosed ?? existedPoll.IsClosed,
            CreatedAt = existedPoll.CreatedAt,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }
}
