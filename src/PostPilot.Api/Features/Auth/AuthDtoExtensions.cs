using PostPilot.Api.Features.Auth.Dtos;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Auth;

public static class AuthDtoExtensions
{
    public static AdminUserDto ToDto(this AdminUser entity)
        => AdminUserDto.FromEntity(entity);
}
