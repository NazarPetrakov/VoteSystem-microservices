namespace AuthService.API.Models;

public class UserToken(string userName, string token)
{
    public string UserName { get; init; } = userName;
    public string Token { get; init; } = token;
}
