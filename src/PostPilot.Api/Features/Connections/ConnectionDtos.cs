namespace PostPilot.Api.Features.Connections;

public sealed record ConnectionStatusDto(
    string Platform,
    bool IsSupported,
    bool IsConfigured,
    bool IsConnected,
    string? DisplayName,
    DateTimeOffset? ExpiresAt);

public sealed record AuthorizationUrlDto(string AuthorizationUrl);
