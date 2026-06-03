using PostPilot.Domain.Common;

namespace PostPilot.Domain.Entities;

public sealed class AdminUser : SoftDeleteEntity
{
    private AdminUser()
    {
    }

    public AdminUser(string email, string passwordHash, string displayName)
    {
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        DisplayName = displayName.Trim();
        IsActive = true;
    }

    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    public void Deactivate()
        => IsActive = false;
}
