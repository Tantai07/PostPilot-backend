using PostPilot.Domain.Common;
using PostPilot.Domain.Enums;

namespace PostPilot.Domain.Entities;

public sealed class AnalyticsSnapshot : SoftDeleteEntity
{
    public Guid ProfileId { get; private set; }
    public SocialPlatform Platform { get; private set; }
    public string MetricName { get; private set; } = string.Empty;
    public long MetricValue { get; private set; }
    public DateTimeOffset CapturedAt { get; private set; }
    public Profile? Profile { get; private set; }
}
