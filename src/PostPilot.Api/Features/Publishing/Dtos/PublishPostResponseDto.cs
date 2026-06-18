namespace PostPilot.Api.Features.Publishing.Dtos;

public sealed class PublishPostResponseDto
{
    public Guid PostId { get; init; }
    public Guid ProfileId { get; init; }
    public string Status { get; init; } = string.Empty;
    public bool IsSuccess { get; init; }
    public DateTimeOffset PublishedAt { get; init; }
    public IReadOnlyCollection<PublishTargetResultDto> Targets { get; init; } = [];
}