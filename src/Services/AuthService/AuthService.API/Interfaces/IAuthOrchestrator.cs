using AuthService.API.Models;

namespace AuthService.API.Interfaces;

public interface IAuthOrchestrator
{
    string GenerateJwtToken(AppUser user, IList<string>? roles = null);
}
