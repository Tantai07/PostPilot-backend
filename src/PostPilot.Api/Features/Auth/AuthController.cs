using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Features.Auth.Dtos;

namespace PostPilot.Api.Features.Auth;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        [FromServices] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var response = await command.ExecuteAsync(request, cancellationToken);
        return response is null ? Unauthorized() : Ok(response);
    }
}
