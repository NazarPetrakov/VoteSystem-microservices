using PollService.API.Helpers;
using PollService.API.Models;

namespace PollService.API.Extensions;

public static class PollOptionDtoExtensions
{
    public static PollOptionResponse ToDto(this PollOption pollOption)
    {
        return new PollOptionResponse(pollOption.Id,
            pollOption.Text,
            pollOption.Order,
            pollOption.PollId,
            pollOption.CreatedAt,
            pollOption.UpdatedAt
        );
    }
    public static PollOption ToEntity(this CreatePollOptionRequest pollOptionCreate)
    {
        return new PollOption()
        {
            Text = pollOptionCreate.Text,
            PollId = pollOptionCreate.PollId
        };
    }
    public static PollOption ToEntity(this CreatePollOptionFromPollDto pollOptionCreate, Guid pollId)
    {
        return new PollOption()
        {
            Text = pollOptionCreate.Text,
            PollId = pollId
        };
    }
    public static PollOption ToEntity(this UpdatePollOptionRequest pollOptionUpdate, PollOption existedOption)
    {
        return new PollOption()
        {
            Id = pollOptionUpdate.Id,
            Text = pollOptionUpdate.Text ?? existedOption.Text,
            Order = pollOptionUpdate.Order ?? existedOption.Order,
            PollId = pollOptionUpdate.PollId ?? existedOption.PollId,
            UpdatedAt = DateTimeOffset.UtcNow,
            CreatedAt = existedOption.CreatedAt
        };
    }
}
