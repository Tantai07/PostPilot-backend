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
            SameSite = environment.IsDevelopment() ? SameSiteMode.Strict : SameSiteMode.None,
            Path = "/",
            Expires = expiresAt,
            IsEssential = true
        };
    }
}
