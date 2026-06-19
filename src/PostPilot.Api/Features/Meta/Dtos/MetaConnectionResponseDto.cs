namespace PostPilot.Api.Features.Meta.Dtos;

public sealed class MetaConnectionResponseDto
{
    public bool IsConnected { get; init; }
    public ConnectedSocialAccountDto? FacebookPage { get; init; }
    public ConnectedSocialAccountDto? InstagramBusiness { get; init; }
}

public sealed class ConnectedSocialAccountDto
{
    public Guid Id { get; init; }
    public string Platform { get; init; } = string.Empty;
    public string PageId { get; init; } = string.Empty;
    public string? IgUserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public bool HasCredential { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
    public bool IsExpired { get; init; }
}
