namespace PostPilot.Api.Features.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "PostPilot";
    public string Audience { get; set; } = "PostPilot";
    public string SigningKey { get; set; } = "development-signing-key-change-me-change-me";
    public int ExpirationMinutes { get; set; } = 120;
}
