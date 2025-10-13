using AuthService.API.Models;
using Common.Contracts.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.API.Data;

public class Seeder(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
{
    private readonly List<AppUser> users = new List<AppUser>()
    {
        new AppUser()
        {
            UserName = "Sasha",
            Email = "sasha@gmail.com",
            NickName = "sashko1234"
        },
        new AppUser()
        {
            UserName = "Nazarii",
            Email = "nazarii@gmail.com",
            NickName = "boss"
        },
        new AppUser()
        {
            UserName = "Vika",
            Email = "vika@gmail.com"
        },
        new AppUser()
        {
            UserName = "admin",
            Email = "admin@gmail.com"
        }
    };
    public async Task SeedUsersAsync()
    {
        if (await userManager.Users.AnyAsync()) return;

        foreach (var user in users)
        {
            if (await userManager.FindByNameAsync(user.UserName!) is null)
            {
                await userManager.CreateAsync(user, "Password1234.");
                await userManager.AddToRoleAsync(user, Roles.Member);
            }
        }

        var adminUser = await userManager.FindByNameAsync("admin");

        if (adminUser is not null)
        {
            await userManager.AddToRoleAsync(adminUser, Roles.Admin);
        }
    }
    public async Task SeedRolesAsync()
    {
        var roles = new AppRole[] { new(Roles.Member), new(Roles.Admin) };

        foreach (var role in roles)
        {
            if (await roleManager.FindByNameAsync(role.Name!) is null)
                await roleManager.CreateAsync(role);
        }
    }
}
