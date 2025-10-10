using System.ComponentModel.DataAnnotations;

namespace AuthService.API.Dtos;

public class UserLoginRequest
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(256, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 256 characters.")]
    public required string UserName { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(64, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 64 characters.")]
    public required string Password { get; set; }
}
