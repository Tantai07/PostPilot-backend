using PostPilot.Domain.Common;
using PostPilot.Domain.Enums;

namespace PostPilot.Domain.Entities;

public sealed class User : SoftDeleteEntity
{
    private User()
    {
    }

    public User(string email, string passwordHash, string displayName, UserRole role)
    {
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        DisplayName = displayName.Trim();
        Role = role;
        IsActive = true;
    }

    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; } = UserRole.User;
    public bool IsActive { get; private set; }

    public void Deactivate()
        => IsActive = false;
}
