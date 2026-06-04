using FluentAssertions;

namespace PostPilot.UnitTests.Security;

public sealed class SeedConfigurationTests
{
    [Fact]
    public void Repository_DoesNotContainAdminSeedConfiguration()
    {
        var repositoryRoot = FindRepositoryRoot();
        var files = Directory
            .EnumerateFiles(repositoryRoot, "*", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}.git{Path.DirectorySeparatorChar}"))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
            .Where(path => !path.EndsWith("SeedConfigurationTests.cs", StringComparison.Ordinal))
            .Where(path => Path.GetFileName(path) is not "appsettings.Local.json")
            .Where(path => path.EndsWith(".cs", StringComparison.Ordinal)
                || path.EndsWith(".json", StringComparison.Ordinal)
                || path.EndsWith(".md", StringComparison.Ordinal)
                || Path.GetFileName(path).StartsWith(".env", StringComparison.Ordinal))
            .ToArray();

        var filesWithSeedConfig = files
            .Where(path => File.ReadAllText(path).Contains("POSTPILOT_SEED_ADMIN", StringComparison.Ordinal))
            .Select(path => Path.GetRelativePath(repositoryRoot, path))
            .ToArray();

        filesWithSeedConfig.Should().BeEmpty();
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, ".git")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new DirectoryNotFoundException("Could not find repository root.");
    }
}
