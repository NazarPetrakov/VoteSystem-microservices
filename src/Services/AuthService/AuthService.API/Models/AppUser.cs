using Microsoft.AspNetCore.Identity;

namespace AuthService.API.Models;

public class AppUser : IdentityUser<int>
{
    public string? NickName { get; set; }
}
