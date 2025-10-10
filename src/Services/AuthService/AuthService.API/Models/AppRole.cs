using Microsoft.AspNetCore.Identity;

namespace AuthService.API.Models;

public class AppRole(string name) : IdentityRole<int>(name)
{

}
