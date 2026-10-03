using System.Diagnostics;

var repositoryRoot = FindRepositoryRoot();
var apiProject = Path.Combine(
    repositoryRoot,
    "src",
    "PostPilot.Api",
    "PostPilot.Api.csproj");

var startInfo = new ProcessStartInfo("dotnet")
{
    WorkingDirectory = repositoryRoot,
    UseShellExecute = false
};

startInfo.ArgumentList.Add("run");
startInfo.ArgumentList.Add("--project");
startInfo.ArgumentList.Add(apiProject);
startInfo.ArgumentList.Add("--no-build");

foreach (var argument in args)
{
    startInfo.ArgumentList.Add(argument);
}

using var apiProcess = Process.Start(startInfo)
    ?? throw new InvalidOperationException("ไม่สามารถเริ่ม PostPilot API ได้");

apiProcess.WaitForExit();
return apiProcess.ExitCode;

static string FindRepositoryRoot()
{
    var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

    while (directory is not null)
    {
        var apiProject = Path.Combine(
            directory.FullName,
            "src",
            "PostPilot.Api",
            "PostPilot.Api.csproj");

        if (File.Exists(apiProject))
        {
            return directory.FullName;
        }

        directory = directory.Parent;
    }

    throw new DirectoryNotFoundException("ไม่พบโฟลเดอร์หลักของ PostPilot");
}
