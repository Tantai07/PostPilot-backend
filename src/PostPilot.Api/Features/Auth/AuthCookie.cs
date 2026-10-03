namespace PostPilot.Api.Features.Auth;

public static class AuthCookie
{
    public const string Name = "postpilot_session";

    public static CookieOptions CreateOptions(
        DateTimeOffset? expiresAt,
        IWebHostEnvironment environment)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !environment.IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = expiresAt,
            IsEssential = true
        };
    }
}
