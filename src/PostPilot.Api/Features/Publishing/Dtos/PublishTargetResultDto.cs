namespace PostPilot.Api.Features.Publishing.Dtos;

public sealed class PublishTargetResultDto
{
    public string Platform { get; init; } = string.Empty;
    public bool IsSuccess { get; init; }
    public string? ExternalPostId { get; init; }
    public string? ErrorMessage { get; init; }
}