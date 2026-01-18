using System.Security.Claims;
using Common.Errors;
using Common.Result;

namespace PollService.API.Extensions;

public static class IdentityExtensions
{
    public static Result<int> GetId(this ClaimsPrincipal principal)
    {
        var principalIdString = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (principalIdString is null)
            return Result.Failure<int>(AuthErrors.Unauthorized);

        return Result.Success(Convert.ToInt32(principalIdString));
    }
}
