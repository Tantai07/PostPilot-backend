using PostPilot.Api.Shared;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Profiles.Dtos;

public sealed class ProfileResponseDto : ResponseDtoBase<Profile>, IFromEntity<Profile, ProfileResponseDto>
{
    private ProfileResponseDto(Profile entity)
        : base(entity)
    {
        OwnerUserId = entity.OwnerUserId;
        Name = entity.Name;
        WebsiteName = entity.WebsiteName;
        DefaultTargets = entity.DefaultTargets;
        UpdatedAt = entity.UpdatedAt ?? entity.CreatedAt;
    }

    public Guid OwnerUserId { get; init; }
    public string Name { get; init; }
    public string? WebsiteName { get; init; }
    public string? DefaultTargets { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }

    public static ProfileResponseDto FromEntity(Profile entity)
        => new(entity);
}
