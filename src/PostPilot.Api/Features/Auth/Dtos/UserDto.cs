using PostPilot.Api.Shared;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;

namespace PostPilot.Api.Features.Auth.Dtos;

public sealed class UserDto : ResponseDtoBase<User>, IFromEntity<User, UserDto>
{
    private UserDto(User entity)
        : base(entity)
    {
        Email = entity.Email;
        DisplayName = entity.DisplayName;
        Role = entity.Role;
    }

    public string Email { get; init; }
    public string DisplayName { get; init; }
    public UserRole Role { get; init; }

    public static UserDto FromEntity(User entity)
        => new(entity);
}
