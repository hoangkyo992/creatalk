namespace Common.Application.Configurations;

public record EmailServiceOptions
{
    public const string ConfigSection = "EmailService";

    public string ServiceName { get; init; } = "Smtp";
    public string FromAddress { get; init; }
    public string FromDisplayName { get; init; } = string.Empty;
    public string Host { get; init; } = "smtp.gmail.com";
    public string AppPassword { get; init; } = string.Empty;
    public int Port { get; init; } = 587;
}