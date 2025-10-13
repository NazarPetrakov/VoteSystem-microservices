namespace Common.Options;

public class JwtOptions
{
    public const string SectionName = "JWT";
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationInDays { get; set; }
    public string SecretKey { get; set; } = string.Empty;
}
