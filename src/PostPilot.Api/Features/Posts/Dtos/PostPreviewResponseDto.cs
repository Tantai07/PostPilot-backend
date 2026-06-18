namespace PostPilot.Api.Features.Posts.Dtos;

public sealed class PostPreviewResponseDto
{
    public Guid ProfileId { get; init; }
    public Guid? CategoryId { get; init; }
    public string Caption { get; init; } = string.Empty;
    public string PreviewCaption { get; init; } = string.Empty;
    public IReadOnlyList<string> CategoryTags { get; init; } = [];
    public IReadOnlyList<PostMediaRequestDto> Media { get; init; } = [];
    public IReadOnlyList<PostTargetRequestDto> Targets { get; init; } = [];
}