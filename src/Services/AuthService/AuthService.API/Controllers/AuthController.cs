using AuthService.API.Dtos;
using AuthService.API.Interfaces;
using AuthService.API.Models;
using Common.Contracts.User;
using Common.Errors;
using Common.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthOrchestrator authOrchestrator,
        IUserPublisher userPublisher,
        UserManager<AppUser> userManager) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            var existingUser = await userManager.FindByNameAsync(request.UserName);
            if (existingUser is not null)
            {
                return BadRequest(AuthErrors.UserAlreadyExists);
            }

            var user = new AppUser
            {
                UserName = request.UserName,
                Email = request.Email,
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var addToRoleResult = await userManager.AddToRoleAsync(user, Roles.Member);
            if (!addToRoleResult.Succeeded)
            {
                return BadRequest(addToRoleResult.Errors);
            }

            await userPublisher.NotifyUserCreatedAsync(user, cancellationToken);

            string token = authOrchestrator.GenerateJwtToken(user);

            return Ok(new UserToken(user.UserName, token));
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginRequest request)
        {
            var user = await userManager.FindByNameAsync(request.UserName);

            if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            {
                return Unauthorized();
            }

            var roles = await userManager.GetRolesAsync(user);

            string token = authOrchestrator.GenerateJwtToken(user, roles);

            return Ok(new UserToken(user.UserName!, token));
        }
    }
}
