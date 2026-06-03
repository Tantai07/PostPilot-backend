using PostPilot.Api.Shared;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Auth.Dtos;

public sealed class AdminUserDto : ResponseDtoBase<AdminUser>, IFromEntity<AdminUser, AdminUserDto>
{
    private AdminUserDto(AdminUser entity)
        : base(entity)
    {
        Email = entity.Email;
        DisplayName = entity.DisplayName;
    }

    public string Email { get; init; }
    public string DisplayName { get; init; }

    public static AdminUserDto FromEntity(AdminUser entity)
        => new(entity);
}
