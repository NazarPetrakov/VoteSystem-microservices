using Common.Result;

namespace Common.Errors;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials =
        new("Auth.InvalidCredentials", "Invalid username or password.");

    public static readonly Error UserAlreadyExists =
        new("Auth.UserAlreadyExists", "A user with this username already exists.");

    public static readonly Error EmailAlreadyExists =
        new("Auth.EmailAlreadyExists", "A user with this email already exists.");

    public static readonly Error UserNotFound =
        new("Auth.UserNotFound", "User not found.");

    public static readonly Error Unauthorized =
        new("Auth.Unauthorized", "You are not authorized to perform this action.");

    public static readonly Error InvalidToken =
        new("Auth.InvalidToken", "The provided token is invalid or expired.");

    public static readonly Error PasswordTooWeak =
        new("Auth.PasswordTooWeak", "The password does not meet security requirements.");

    public static readonly Error RegistrationFailed =
        new("Auth.RegistrationFailed", "User registration failed. Please try again later.");

    public static readonly Error EmailNotConfirmed =
        new("Auth.EmailNotConfirmed", "You must confirm your email before logging in.");

    public static readonly Error Forbidden =
        new("Auth.Forbidden", "You are not allowed.");

}
