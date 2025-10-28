using Common.Contracts.Params;

namespace PollService.API.Helpers;

public class PollQueryParams : QueryParams
{
    public string? Topic { get; set; }
    public bool? IsClosed { get; set; }
    public bool? IsExpired { get; set; }
}
