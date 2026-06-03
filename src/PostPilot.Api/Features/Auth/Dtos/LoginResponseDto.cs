namespace PostPilot.Api.Features.Auth.Dtos;

public sealed class LoginResponseDto
{
    public required string AccessToken { get; init; }
    public required string TokenType { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }
    public required AdminUserDto AdminUser { get; init; }
}
