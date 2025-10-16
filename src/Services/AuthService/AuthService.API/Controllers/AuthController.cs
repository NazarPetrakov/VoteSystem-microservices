using AuthService.API.Dtos;
using AuthService.API.Interfaces;
using AuthService.API.Models;
using Common.Contracts.User;
using Common.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthOrchestrator authOrchestrator) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> RegisterMember(UserRegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await authOrchestrator.RegisterUserAsync(request, false, ModelState, cancellationToken);

            return result.Match<UserToken, IActionResult>
            (
                onSuccess: Ok,
                onFailure: BadRequest
            );
        }
        [Authorize(AuthenticationSchemes = "Bearer", Roles = Roles.Admin)]
        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin(UserRegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await authOrchestrator.RegisterUserAsync(request, true, ModelState, cancellationToken);

            return result.Match<UserToken, IActionResult>
            (
                onSuccess: Ok,
                onFailure: BadRequest
            );
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var result = await authOrchestrator.LoginUserAsync(request);

            return result.Match<UserToken, IActionResult>
            (
                onSuccess: Ok,
                onFailure: Unauthorized
            );
        }
    }
}
