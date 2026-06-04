using PostPilot.Api.Features.Auth.Dtos;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Auth;

public static class AuthDtoExtensions
{
    public static UserDto ToDto(this User entity)
        => UserDto.FromEntity(entity);
}
