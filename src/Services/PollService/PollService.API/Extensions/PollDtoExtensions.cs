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
            poll.CreatedByUserId,
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
            CreatedByUserId = pollCreate.CreatedByUserId,
            IsClosed = pollCreate.IsClosed,
        };
    }
    public static Poll ToEntity(this UpdatePollRequest pollUpdate, DateTimeOffset createTime)
    {
        return new Poll()
        {
            Id = pollUpdate.Id,
            Title = pollUpdate.Title,
            Description = pollUpdate.Description,
            CreatedByUserId = pollUpdate.CreatedByUserId,
            IsClosed = pollUpdate.IsClosed,
            CreatedAt = createTime,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }
}
