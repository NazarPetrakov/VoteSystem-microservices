using System.Security.Claims;
using System.Text;
using AuthService.API.Dtos;
using AuthService.API.Interfaces;
using AuthService.API.Models;
using Common.Contracts.User;
using Common.Errors;
using Common.Extensions;
using Common.Options;
using Common.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.API.Orchestrators;

public class AuthOrchestrator(IOptions<JwtOptions> jwtOptions,
    UserManager<AppUser> userManager,
    IUserPublisher userPublisher) : IAuthOrchestrator
{
    public async Task<Result<UserToken>> LoginUserAsync(UserLoginRequest request)
    {
        var user = await userManager.FindByNameAsync(request.UserName);

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Result.Failure<UserToken>(AuthErrors.InvalidCredentials);
        }

        var roles = await userManager.GetRolesAsync(user);

        string token = GenerateJwtToken(user, roles);

        return Result.Success(new UserToken(user.UserName!, token));
    }
    public async Task<Result<UserToken>> RegisterUserAsync(UserRegisterRequest request,
        bool isAdmin,
        ModelStateDictionary modelState,
        CancellationToken cancellationToken)
    {
        if (!modelState.IsValid)
        {
            return Result.Failure<UserToken>(modelState.GetErrors());
        }

        var existingUser = await userManager.FindByNameAsync(request.UserName);
        if (existingUser is not null)
        {
            return Result.Failure<UserToken>(AuthErrors.UserAlreadyExists);
        }

        var user = new AppUser
        {
            UserName = request.UserName,
            Email = request.Email,
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return Result.Failure<UserToken>(result.Errors
                .Select(ie => new Error(ie.Code, ie.Description)));
        }

        var roles = new List<string>() { Roles.Member };

        if (isAdmin)
        {
            roles.Add(Roles.Admin);
        }

        var addToRoleResult = await userManager.AddToRolesAsync(user, roles);
        if (!addToRoleResult.Succeeded)
        {
            return Result.Failure<UserToken>(addToRoleResult.Errors
                .Select(ie => new Error(ie.Code, ie.Description)));
        }

        await userPublisher.NotifyUserCreatedAsync(user, cancellationToken);

        string token = GenerateJwtToken(user, roles);

        return Result.Success(new UserToken(user.UserName, token));
    }
    public string GenerateJwtToken(AppUser user, IList<string>? roles = null)
    {
        var jwtOptionsValue = jwtOptions.Value;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? "")
        };

        if (roles is not null)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptionsValue.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(jwtOptionsValue.ExpirationInDays),
            SigningCredentials = creds,
            Issuer = jwtOptionsValue.Issuer,
            Audience = jwtOptionsValue.Audience,
        };

        var tokenHandler = new JsonWebTokenHandler();

        string accessToken = tokenHandler.CreateToken(tokenDescriptor);

        return accessToken;
    }
}
