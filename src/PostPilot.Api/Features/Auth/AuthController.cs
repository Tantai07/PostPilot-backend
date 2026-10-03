using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Features.Auth.Dtos;
using PostPilot.Api.Features.Auth.Queries;
using PostPilot.Infrastructure.Auth;

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
        [FromServices] IWebHostEnvironment environment,
        CancellationToken cancellationToken)
    {
        var response = await command.ExecuteAsync(request, cancellationToken);
        if (response is null)
        {
            return Unauthorized();
        }

        Response.Cookies.Append(
            AuthCookie.Name,
            response.AccessToken,
            AuthCookie.CreateOptions(response.ExpiresAt, environment));

        return Ok(response);
    }

    [Authorize]
    [HttpGet("session")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Session(
        [FromServices] CurrentSessionQuery query,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var user = await query.ExecuteAsync(currentUser.UserId.Value, cancellationToken);
        if (user is null)
        {
            return Unauthorized();
        }

        var expiresAt = DateTimeOffset.UtcNow;
        var expiresClaim = User.FindFirstValue(JwtRegisteredClaimNames.Exp);
        if (long.TryParse(expiresClaim, out var expiresUnixTime))
        {
            expiresAt = DateTimeOffset.FromUnixTimeSeconds(expiresUnixTime);
        }

        return Ok(new LoginResponseDto
        {
            AccessToken = string.Empty,
            ExpiresAt = expiresAt,
            User = user
        });
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout([FromServices] IWebHostEnvironment environment)
    {
        Response.Cookies.Delete(AuthCookie.Name, AuthCookie.CreateOptions(null, environment));
        return NoContent();
    }
}
