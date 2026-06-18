using PostPilot.Api.Features.Media.Dtos;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Media;

public static class MediaDtoExtensions
{
    public static MediaUploadResponseDto ToDto(this MediaAsset entity)
        => MediaUploadResponseDto.FromEntity(entity);
}