namespace Common.Options;

public class RabbitMqOptions
{
    public const string RabbitMq = "RabbitMq";
    public string Host { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
