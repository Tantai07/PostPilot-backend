using PostPilot.Api.Features.Media.Dtos;

namespace PostPilot.Api.Features.Posts.Dtos;

public sealed class PostPreviewResponseDto
{
    public Guid ProfileId { get; init; }
    public Guid? CategoryId { get; init; }
    public string Caption { get; init; } = string.Empty;
    public string PreviewCaption { get; init; } = string.Empty;
    public IReadOnlyCollection<string> CategoryTags { get; init; } = [];
    public IReadOnlyCollection<MediaUploadResponseDto> Media { get; init; } = [];
    public IReadOnlyCollection<string> TargetPlatforms { get; init; } = [];
}