namespace PostPilot.Api.Features.Publishing;

public sealed class PublishingOptions
{
    public string Provider { get; set; } = "Mock";
    public string GraphApiVersion { get; set; } = "v20.0";
}
