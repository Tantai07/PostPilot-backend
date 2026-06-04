using PostPilot.Api.Features.Auth.Dtos;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Auth;

public interface IJwtTokenService
{
    LoginResponseDto CreateLoginResponse(User user);
}
