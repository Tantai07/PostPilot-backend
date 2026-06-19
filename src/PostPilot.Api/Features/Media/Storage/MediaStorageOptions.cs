namespace PostPilot.Api.Features.Media.Storage;

public sealed class MediaStorageOptions
{
    public string Provider { get; set; } = "Local";
    public string CloudinaryCloudName { get; set; } = string.Empty;
    public string CloudinaryApiKey { get; set; } = string.Empty;
    public string CloudinaryApiSecret { get; set; } = string.Empty;
    public string CloudinaryFolder { get; set; } = "postpilot";
}
