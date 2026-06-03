namespace PostPilot.Infrastructure.Auth;

public interface ICurrentUserContext
{
    Guid? UserId { get; }
    string? Email { get; }
}
