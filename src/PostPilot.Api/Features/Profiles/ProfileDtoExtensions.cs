using PostPilot.Api.Features.Profiles.Dtos;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Profiles;

public static class ProfileDtoExtensions
{
    public static ProfileResponseDto ToDto(this Profile entity)
        => ProfileResponseDto.FromEntity(entity);
}
