using System.Text.Json.Serialization;

namespace PostPilot.Api.Features.Auth.Dtos;

public sealed class LoginResponseDto
{
    [JsonIgnore]
    public string AccessToken { get; init; } = string.Empty;
    public required DateTimeOffset ExpiresAt { get; init; }
    public required UserDto User { get; init; }
}
