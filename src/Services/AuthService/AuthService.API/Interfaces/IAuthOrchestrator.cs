using AuthService.API.Dtos;
using AuthService.API.Models;
using Common.Result;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AuthService.API.Interfaces;

public interface IAuthOrchestrator
{
    string GenerateJwtToken(AppUser user, IList<string>? roles = null);
    Task<Result<UserToken>> LoginUserAsync(UserLoginRequest request);
    Task<Result<UserToken>> RegisterUserAsync(UserRegisterRequest request,
        bool isAdmin,
        ModelStateDictionary modelState,
        CancellationToken cancellationToken);
}
