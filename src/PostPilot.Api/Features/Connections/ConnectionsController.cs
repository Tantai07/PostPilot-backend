using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Api.Features.Connections;

[ApiController]
[Authorize]
public sealed class ConnectionsController : ControllerBase
{
    [HttpGet("api/profiles/{profileId:guid}/connections")]
    public async Task<ActionResult<IReadOnlyCollection<ConnectionStatusDto>>> List(
        Guid profileId,
        [FromServices] OAuthConnectionService service,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null) return Unauthorized();
        var statuses = await service.GetStatusesAsync(currentUser.UserId.Value, profileId, cancellationToken);
        return statuses is null ? NotFound() : Ok(statuses);
    }

    [HttpGet("api/profiles/{profileId:guid}/connections/{platform}/authorize")]
    public async Task<ActionResult<AuthorizationUrlDto>> Authorize(
        Guid profileId,
        string platform,
        [FromServices] OAuthConnectionService service,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null) return Unauthorized();
        if (!OAuthConnectionService.TryParsePlatform(platform, out var parsed)) return BadRequest("Unknown platform.");
        try
        {
            var url = await service.CreateAuthorizationUrlAsync(currentUser.UserId.Value, profileId, parsed, cancellationToken);
            return url is null ? NotFound() : Ok(new AuthorizationUrlDto(url));
        }
        catch (NotSupportedException exception) { return BadRequest(exception.Message); }
        catch (InvalidOperationException exception) { return BadRequest(exception.Message); }
    }

    [HttpDelete("api/profiles/{profileId:guid}/connections/{platform}")]
    public async Task<IActionResult> Disconnect(
        Guid profileId,
        string platform,
        [FromServices] OAuthConnectionService service,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null) return Unauthorized();
        if (!OAuthConnectionService.TryParsePlatform(platform, out var parsed)) return BadRequest("Unknown platform.");
        return await service.DisconnectAsync(currentUser.UserId.Value, profileId, parsed, cancellationToken) ? NoContent() : NotFound();
    }

    [AllowAnonymous]
    [HttpGet("api/oauth/{platform}/callback")]
    public async Task<IActionResult> Callback(
        string platform,
        [FromQuery] string? code,
        [FromQuery] string? state,
        [FromQuery] string? error,
        [FromServices] OAuthConnectionService service,
        CancellationToken cancellationToken)
    {
        var status = "error";
        var message = error ?? "การเชื่อมต่อไม่สำเร็จ";
        var completedPlatform = platform;
        if (string.IsNullOrWhiteSpace(error) && !string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(state))
        {
            try
            {
                var callbackParameters = Request.Query.ToDictionary(
                    item => item.Key,
                    item => item.Value.ToString(),
                    StringComparer.OrdinalIgnoreCase);
                var callback = new OAuthCallbackData(code, callbackParameters);
                var payload = await service.CompleteAsync(callback, state, cancellationToken);
                status = "success";
                message = "เชื่อมต่อสำเร็จ สามารถปิดหน้าต่างนี้ได้";
                completedPlatform = payload.Platform.ToString();
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                message = exception is InvalidOperationException or NotSupportedException
                    ? exception.Message
                    : "ผู้ให้บริการไม่สามารถดำเนินการเชื่อมต่อได้ กรุณาลองอีกครั้ง";
            }
        }

        var payloadJson = JsonSerializer.Serialize(new { type = "postpilot-oauth", status, platform = completedPlatform, message });
        var originJson = JsonSerializer.Serialize(service.FrontendOrigin);
        var safeMessage = HtmlEncoder.Default.Encode(message);
        return Content($"""
            <!doctype html><html lang="th"><meta charset="utf-8"><title>PostPilot</title>
            <body style="font-family:system-ui;padding:32px"><p>{safeMessage}</p>
            <script>window.opener?.postMessage({payloadJson}, {originJson}); if ({JsonSerializer.Serialize(status)} === 'success') window.close();</script>
            </body></html>
            """, "text/html; charset=utf-8");
    }
}
