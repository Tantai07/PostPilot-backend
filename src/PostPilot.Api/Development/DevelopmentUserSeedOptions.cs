namespace PostPilot.Api.Development;

public sealed record DevelopmentUserSeedOptions(
    string UserEmail,
    string UserPassword,
    string AdminEmail,
    string AdminPassword)
{
    public static DevelopmentUserSeedOptions FromConfiguration(IConfiguration configuration)
    {
        var options = new DevelopmentUserSeedOptions(
            Require(configuration, "POSTPILOT_TEST_USER_EMAIL"),
            Require(configuration, "POSTPILOT_TEST_USER_PASSWORD"),
            Require(configuration, "POSTPILOT_TEST_ADMIN_EMAIL"),
            Require(configuration, "POSTPILOT_TEST_ADMIN_PASSWORD"));

        if (string.Equals(options.UserEmail, options.AdminEmail, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Test user and admin email addresses must be different.");
        }

        return options;
    }

    private static string Require(IConfiguration configuration, string key)
    {
        var value = configuration[key];
        return !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new InvalidOperationException($"Test seed configuration is missing. Set {key}.");
    }
}
