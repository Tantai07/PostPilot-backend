using System.Text;

namespace PostPilot.Infrastructure.Storage;

public sealed class DevelopmentTokenEncryptionService : ITokenEncryptionService
{
    public string Encrypt(string plainText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plainText);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
    }

    public string Decrypt(string cipherText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cipherText);
        return Encoding.UTF8.GetString(Convert.FromBase64String(cipherText));
    }
}
