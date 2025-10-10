using System.Security.Claims;
using System.Text;
using AuthService.API.Interfaces;
using AuthService.API.Models;
using AuthService.API.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.API.Orchestrators;

public class AuthOrchestrator(IOptions<JwtOptions> jwtOptions) : IAuthOrchestrator
{
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
