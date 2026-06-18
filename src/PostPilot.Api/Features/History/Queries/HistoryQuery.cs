using PostPilot.Api.Shared;

namespace PostPilot.Api.Features.History.Queries;

public sealed class HistoryQuery : BaseQuery
{
    public string? Platform { get; set; }
    public string? Status { get; set; }
    public Guid? CategoryId { get; set; }
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }
}