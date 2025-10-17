namespace AuthService.API.Models;

public class UserToken(int userId, string userName, string token)
{
    public int UserId { get; set; } = userId;
    public string UserName { get; init; } = userName;
    public string Token { get; init; } = token;
}
